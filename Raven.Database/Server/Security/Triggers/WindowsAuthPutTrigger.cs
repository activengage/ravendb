using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Database.Server.Security.Windows;

namespace Raven35.Database.Server.Security.Triggers
{
    class WindowsAuthPutTrigger : AbstractPutTrigger
    {
        public override VetoResult AllowPut(string key, Raven35.Json.Linq.RavenJObject document, Raven35.Json.Linq.RavenJObject metadata, TransactionInformation transactionInformation)
        {
            if (key == "Raven/Authorization/WindowsSettings" && Authentication.IsEnabled == false)
                return VetoResult.Deny("Cannot setup Windows Authentication without a valid commercial license.");

            return VetoResult.Allowed;
        }

        public override void AfterPut(string key, Raven35.Json.Linq.RavenJObject document, Raven35.Json.Linq.RavenJObject metadata, Etag etag, Raven35.Abstractions.Data.TransactionInformation transactionInformation)
        {
            if (key == "Raven/Authorization/WindowsSettings")
                WindowsRequestAuthorizer.InvokeWindowsSettingsChanged();
            base.AfterPut(key, document, metadata, etag, transactionInformation);
        }
    }
}
