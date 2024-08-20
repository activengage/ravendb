using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Tests.Common.Triggers
{
    public class HideVirtuallyDeletedDocument : AbstractReadTrigger
    {
        public override ReadVetoResult AllowRead(string key, RavenJObject metadata, ReadOperation operation, TransactionInformation transactionInformation)
        {
            if (operation != ReadOperation.Index)
                return ReadVetoResult.Allowed;
            if (metadata.ContainsKey("Deleted") == false)
                return ReadVetoResult.Allowed;
            return ReadVetoResult.Ignore;
        }
    }
}
