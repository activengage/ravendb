using System;
using System.ComponentModel.Composition;
using System.IO;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Quotas.Size.Triggers
{
    [InheritedExport(typeof(AbstractAttachmentPutTrigger))]
    [ExportMetadata("Bundle", "Quotas")]
    [Obsolete("Use RavenFS instead.")]
    public class DatabaseSizeQuotaForAttachmentsPutTrigger : AbstractAttachmentPutTrigger
    {
        public override VetoResult AllowPut(string key, Stream data, RavenJObject metadata)
        {
            return SizeQuotaConfiguration.GetConfiguration(Database).AllowPut();
        }
    }
}
