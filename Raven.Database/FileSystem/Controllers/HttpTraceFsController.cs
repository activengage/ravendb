using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Raven35.Database.Server.Connections;
using Raven35.Database.Server.WebApi.Attributes;

namespace Raven35.Database.FileSystem.Controllers
{
    public class HttpTraceFsController : BaseFileSystemApiController
    {
        [HttpGet]
        [RavenRoute("fs/{fileSystemName}/traffic-watch/events")]
        public HttpResponseMessage HttpTrace()
        {
            var traceTransport = new HttpTracePushContent();
            traceTransport.Headers.ContentType = new MediaTypeHeaderValue("text/event-stream");

            RequestManager.RegisterResourceHttpTraceTransport(traceTransport, FileSystemName);

            return new HttpResponseMessage { Content = traceTransport };
        }
    }
}
