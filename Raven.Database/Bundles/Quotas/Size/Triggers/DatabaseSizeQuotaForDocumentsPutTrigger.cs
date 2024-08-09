using System.ComponentModel.Composition;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Quotas.Size.Triggers
{

    [InheritedExport(typeof(AbstractPutTrigger))]
    [ExportMetadata("Bundle", "Quotas")]
    public class DatabaseSizeQuotaForDocumentsPutTrigger : AbstractPutTrigger
    {
        public override VetoResult AllowPut(string key, RavenJObject document, RavenJObject metadata,
                                            TransactionInformation transactionInformation)
        {
            return SizeQuotaConfiguration.GetConfiguration(Database).AllowPut();
        }

    }
}
