using System;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Database.Server;
using Raven35.Json.Linq;

namespace Raven35.Tests.Triggers.Bugs
{
    public class AuditTrigger : AbstractPutTrigger
    {
        // Metadata delivered over http as http header, so the datetime format used over the http header is 'r' which doesn't support milliseconds. 
        public static DateTime CreatedAtDateTime = new DateTime(2011, 02, 19, 4, 8, 14, millisecond: 0);

        public override void OnPut(string key, RavenJObject jsonReplicationDocument, RavenJObject metadata, TransactionInformation transactionInformation)
        {
            if (AuditContext.IsInAuditContext)
                return;

            using (AuditContext.Enter())
            {
                if (metadata.Value<string>("Raven-Entity-Name") == "People")
                {
                    if (metadata["CreatedByPersonId"] == null)
                    {
                        metadata["CreatedByPersonId"] = CurrentOperationContext.Headers.Value.Value["CurrentUserPersonId"];
                        metadata["CreatedDate"] = CreatedAtDateTime;
                    }
                    else
                    {
                        metadata["LastUpdatedPersonId"] = CurrentOperationContext.Headers.Value.Value["CurrentUserPersonId"];
                        metadata["LastUpdatedDate"] = CreatedAtDateTime;
                    }
                }
            }
        }
    }
}
