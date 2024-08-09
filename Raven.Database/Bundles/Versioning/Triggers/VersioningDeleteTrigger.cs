//-----------------------------------------------------------------------
// <copyright file="VersioningDeleteTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
// using Microsoft.VisualBasic.Logging;
using Raven35.Abstractions.Data;
using Raven35.Database.Impl;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Versioning.Triggers
{
    [InheritedExport(typeof(AbstractDeleteTrigger))]
    [ExportMetadata("Bundle", "Versioning")]
    public class VersioningDeleteTrigger : AbstractDeleteTrigger
    {
        readonly ThreadLocal<Dictionary<string, RavenJObject>> versionInformer 
            = new ThreadLocal<Dictionary<string, RavenJObject>>(() => new Dictionary<string, RavenJObject>());

        public override VetoResult AllowDelete(string key, TransactionInformation transactionInformation)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                var document = Database.Documents.Get(key, transactionInformation);
                if (document == null)
                    return VetoResult.Allowed;

                versionInformer.Value[key] = document.Metadata;
                if (document.Metadata.Value<string>(VersioningUtil.RavenDocumentRevisionStatus) != "Historical")
                    return VetoResult.Allowed;

                if (Database.ChangesToRevisionsAllowed() == false &&
                    Database.IsVersioningActive(document.Metadata))
                {
                    var revisionPos = key.LastIndexOf("/revisions/", StringComparison.OrdinalIgnoreCase);
                    if (revisionPos != -1)
                    {
                        var parentKey = key.Remove(revisionPos);
                        var parentDoc = Database.Documents.Get(parentKey, transactionInformation);
                        if (parentDoc == null)
                            return VetoResult.Allowed;
                    }

                    return VetoResult.Deny("Deleting a historical revision is not allowed");
                }

                return VetoResult.Allowed;
            }
        }

        public override void AfterDelete(string key, TransactionInformation transactionInformation)
        {
            try
            {
                RavenJObject jObject = null;
                versionInformer.Value.TryGetValue(key, out jObject);

                var versioningConfig = Database.GetDocumentVersioningConfiguration(jObject);
    
                using (Database.DisableAllTriggersForCurrentThread())
                using (Database.DocumentLock.Lock())
                {
                    Database.TransactionalStorage.Batch(accessor =>
                    {
                        using (DocumentCacher.SkipSetDocumentsInDocumentCache())
                        {
                            foreach (var jsonDocument in accessor.Documents.GetDocumentsWithIdStartingWith(key + "/revisions/", 0, int.MaxValue, null))
                            {
                                if (jsonDocument == null)
                                    continue;
                                if (versioningConfig != null && versioningConfig.PurgeOnDelete)
                                {
                                    Database.Documents.Delete(jsonDocument.Key, null, transactionInformation);
                                }
                                else
                                {
                                    jsonDocument.Metadata.Remove(Constants.RavenReadOnly);
                                    accessor.Documents.AddDocument(jsonDocument.Key, jsonDocument.Etag, jsonDocument.DataAsJson, jsonDocument.Metadata);
                                }
                            }
                        }
                    });
                }
            }
            finally
            {
                versionInformer.Value.Remove(key);
            }
        }

        public override void Dispose()
        {
            versionInformer.Dispose();
            base.Dispose();
        }
    }
}
