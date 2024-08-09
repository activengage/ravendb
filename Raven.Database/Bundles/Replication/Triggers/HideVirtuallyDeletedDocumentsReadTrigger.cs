//-----------------------------------------------------------------------
// <copyright file="HideVirtuallyDeletedDocumentsReadTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Replication.Triggers
{
    [ExportMetadata("Bundle", "Replication")]
    [ExportMetadata("Order", 10000)]
    [InheritedExport(typeof(AbstractReadTrigger))]
    public class HideVirtuallyDeletedDocumentsReadTrigger : AbstractReadTrigger
    {
        public override ReadVetoResult AllowRead(string key, RavenJObject metadata, ReadOperation operation,
                                                 TransactionInformation transactionInformation)
        {
            if(metadata == null)
                return ReadVetoResult.Allowed; // this is a projection, it is allowed
            RavenJToken value;
            if (metadata.TryGetValue("Raven-Delete-Marker", out value))
                return ReadVetoResult.Ignore;
            return ReadVetoResult.Allowed;
        }
    }
}
