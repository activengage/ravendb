using System;

using Raven35.Database.Common;
using Raven35.Database.Server.Controllers;

namespace Raven35.Database.Server
{
    public class RequestWebApiEventArgs : EventArgs
    {
        public string TenantId { get; set; }
        public bool IgnoreRequest { get; set; }
        public RavenBaseApiController Controller { get; set; }

        public IResourceStore Resource { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}
