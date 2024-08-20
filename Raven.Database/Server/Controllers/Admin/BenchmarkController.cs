using System.Net;
using System.Net.Http;
using System.Web.Http;
using Raven35.Database.Server.WebApi.Attributes;


namespace Raven35.Database.Server.Controllers.Admin
{
    public class BenchmarkController : BaseDatabaseApiController
    {
        [HttpGet]
        [RavenRoute("Benchmark/EmptyMessage")]
        public HttpResponseMessage EmptyMessageTest()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
