using System;
using System.ComponentModel.Composition;
using Raven35.Database.Plugins;

namespace Raven35.Bundles.Quotas.Size.Triggers
{
    [InheritedExport(typeof(AbstractAttachmentDeleteTrigger))]
    [ExportMetadata("Bundle", "Quotas")]
    [Obsolete("Use RavenFS instead.")]
    public class DatabaseSizeAttachmentDeleteTrigger : AbstractAttachmentDeleteTrigger
    {
        public override void AfterDelete(string key)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                SizeQuotaConfiguration.GetConfiguration(Database).AfterDelete();				
            }
        }
    }
}
