using System.ComponentModel.Composition;
using Raven35.Database.Plugins;

namespace Raven35.Bundles.Quotas.Size.Triggers
{
    [InheritedExport(typeof(AbstractDeleteTrigger))]
    [ExportMetadata("Bundle", "Quotas")]
    public class DatabaseSizeDocumentDeleteTrigger : AbstractDeleteTrigger
    {
        public override void AfterDelete(string key, Abstractions.Data.TransactionInformation transactionInformation)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                SizeQuotaConfiguration.GetConfiguration(Database).AfterDelete();
            }
        }
    }
}
