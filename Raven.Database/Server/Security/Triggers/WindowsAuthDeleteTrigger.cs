using Raven35.Database.Plugins;
using Raven35.Database.Server.Security.Windows;
using Raven35.Json.Linq;

namespace Raven35.Database.Server.Security.Triggers
{
    class WindowsAuthDeleteTrigger : AbstractDeleteTrigger
    {
        public override void AfterDelete(string key, Raven35.Abstractions.Data.TransactionInformation transactionInformation,RavenJObject metadata)
        {
            if (key == "Raven/Authorization/WindowsSettings")
                WindowsRequestAuthorizer.InvokeWindowsSettingsChanged();
            base.AfterDelete(key, transactionInformation);
        }
    }
}
