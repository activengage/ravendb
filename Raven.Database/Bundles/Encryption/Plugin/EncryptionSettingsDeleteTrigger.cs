using System.ComponentModel.Composition;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;

namespace Raven35.Bundles.Encryption.Plugin
{
    [InheritedExport(typeof(AbstractDeleteTrigger))]
    [ExportMetadata("Order", 10000)]
    [ExportMetadata("Bundle", "Encryption")]
    public class EncryptionSettingsDeleteTrigger : AbstractDeleteTrigger
    {
        public override VetoResult AllowDelete(string key, TransactionInformation transactionInformation)
        {
            if (key == Constants.InResourceKeyVerificationDocumentName)
                return VetoResult.Deny("Cannot delete the encryption verification document.");

            return base.AllowDelete(key, transactionInformation);
        }
    }
}
