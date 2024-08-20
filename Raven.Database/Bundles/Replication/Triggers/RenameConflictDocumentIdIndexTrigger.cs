// -----------------------------------------------------------------------
//  <copyright file="RenameConflictDocumentIdIndexTrigger.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Lucene.Net.Documents;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;

namespace Raven35.Database.Bundles.Replication.Triggers
{
    [ExportMetadata("Bundle", "Replication")]
    [ExportMetadata("Order", 10000)]
    [InheritedExport(typeof(AbstractIndexUpdateTrigger))]
    public class RenameConflictDocumentIdIndexTrigger : AbstractIndexUpdateTrigger
    {
        public override AbstractIndexUpdateTriggerBatcher CreateBatcher(int indexId)
        {
            var index = Database.IndexStorage.GetIndexInstance(indexId);
            
            if (index == null || index.IsMapReduce)
                return null;

            return new Batcher();
        }

        public class Batcher : AbstractIndexUpdateTriggerBatcher
        {
            public override void OnIndexEntryCreated(string entryKey, Document document)
            {
                var conflitsIndex = entryKey.IndexOf("/conflicts/", StringComparison.Ordinal);

                if (conflitsIndex <= 0)
                    return;

                var documentIdField = document.GetField(Constants.DocumentIdFieldName);

                if(documentIdField == null)
                    return;

                documentIdField.SetValue(entryKey.Substring(0, conflitsIndex));
            }
        }
    }
}
