using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Bundles.Encryption.Settings;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Encryption.Plugin
{
    [InheritedExport(typeof(AbstractPutTrigger))]
    [ExportMetadata("Order", 10000)]
    [ExportMetadata("Bundle", "Encryption")]
    public class EncryptionSettingsPutTrigger : AbstractPutTrigger
    {
        public override VetoResult AllowPut(string key, RavenJObject document, RavenJObject metadata, TransactionInformation transactionInformation)
        {
            if (key == Constants.InResourceKeyVerificationDocumentName)
            {
                if (Database == null) // we haven't been initialized yet
                    return VetoResult.Allowed;

                if (Database.Documents.Get(key, null) != null)
                    return VetoResult.Deny("The encryption verification document already exists and cannot be overwritten.");
            }

            return VetoResult.Allowed;
        }
    }
}
