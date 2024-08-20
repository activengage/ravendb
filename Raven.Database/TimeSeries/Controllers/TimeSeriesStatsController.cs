using System.Net;
using System.Net.Http;
using System.Web.Http;
using Raven35.Database.Server.WebApi.Attributes;

namespace Raven35.Database.TimeSeries.Controllers
{
    public class TimeSeriesStatsController : BaseTimeSeriesApiController
    {
        [HttpGet]
        [RavenRoute("ts/{timeSeriesName}/stats")]
        public HttpResponseMessage TimeSeriesStats()
        {
            using (var reader = TimeSeries.CreateReader())
            {
                var stats = TimeSeries.CreateStats(reader);
                return GetMessageWithObject(stats);
            }
        }
    }
}
