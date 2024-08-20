using System.ComponentModel.Composition;
using Raven35.Bundles.Quotas.Size;
using Raven35.Database.Plugins;

namespace Raven35.Bundles.Quotas.Documents.Triggers
{

    [InheritedExport(typeof(AbstractDeleteTrigger))]
    [ExportMetadata("Bundle", "Quotas")]
    public class DatabaseCountDocumentDeleteTrigger : AbstractDeleteTrigger
    {
        public override void AfterDelete(string key, Abstractions.Data.TransactionInformation transactionInformation)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                DocQuotaConfiguration.GetConfiguration(Database).AfterDelete();
            }
        }
    }
}
