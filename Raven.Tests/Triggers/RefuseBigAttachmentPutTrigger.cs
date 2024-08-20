using System.IO;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Tests.Triggers
{
    public class RefuseBigAttachmentPutTrigger : AbstractAttachmentPutTrigger
    {
        public override VetoResult AllowPut(string key, Stream data, RavenJObject metadata)
        {
            if (data.Length > 4)
                return VetoResult.Deny("Attachment is too big");

            return VetoResult.Allowed;
        }
    }
}
