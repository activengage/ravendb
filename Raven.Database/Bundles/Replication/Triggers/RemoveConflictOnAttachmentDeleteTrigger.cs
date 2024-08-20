//-----------------------------------------------------------------------
// <copyright file="RemoveConflictOnPutTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;
using System.Linq;
using Raven35.Abstractions.Extensions;

namespace Raven35.Bundles.Replication.Triggers
{
    [ExportMetadata("Bundle", "Replication")]
    [ExportMetadata("Order", 10001)]
    [InheritedExport(typeof(AbstractAttachmentDeleteTrigger))]
    [Obsolete("Use RavenFS instead.")]
    public class RemoveConflictOnAttachmentDeleteTrigger : AbstractAttachmentDeleteTrigger
    {
        public override void OnDelete(string key)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                var oldVersion = Database.Attachments.GetStatic(key);
                if(oldVersion == null)
                    return;

                if (oldVersion.Metadata[Constants.RavenReplicationConflict] == null)
                    return;

                var conflictData = oldVersion.Data().ToJObject();
                var conflicts = conflictData.Value<RavenJArray>("Conflicts");
                if(conflicts == null)
                    return;
                foreach (var prop in conflicts)
                {
                    Database.Attachments.DeleteStatic(prop.Value<string>(), null);
                }
            }
        }
    }
}
