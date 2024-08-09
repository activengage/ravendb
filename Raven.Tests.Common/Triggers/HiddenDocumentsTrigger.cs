using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Tests.Common.Triggers
{
    public class HiddenDocumentsTrigger : AbstractReadTrigger
    {
        public override ReadVetoResult AllowRead(string key, RavenJObject metadata, ReadOperation operation, TransactionInformation transactionInformation)
        {
            if (operation == ReadOperation.Index)
                return ReadVetoResult.Allowed;
            var name = metadata["hidden"];
            if (name != null && name.Value<bool>())
            {
                return ReadVetoResult.Ignore;
            }
            return ReadVetoResult.Allowed;
        }
    }
}
