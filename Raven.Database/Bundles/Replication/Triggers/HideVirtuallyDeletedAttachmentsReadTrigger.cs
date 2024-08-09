//-----------------------------------------------------------------------
// <copyright file="HideVirtuallyDeletedAttachmentsReadTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.IO;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Replication.Triggers
{
    [ExportMetadata("Bundle", "Replication")]
    [ExportMetadata("Order", 10000)]
    [InheritedExport(typeof(AbstractAttachmentReadTrigger))]
    [Obsolete("Use RavenFS instead.")]
    public class HideVirtuallyDeletedAttachmentsReadTrigger : AbstractAttachmentReadTrigger
    {
        public override ReadVetoResult AllowRead(string key, Stream data, RavenJObject metadata, ReadOperation operation)
        {
            RavenJToken value;
            if (metadata.TryGetValue("Raven-Delete-Marker", out value))
                return ReadVetoResult.Ignore;
            return ReadVetoResult.Allowed;
        }
    }
}
