using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Raven35.Abstractions.Data;
using Raven35.Database.Server.Controllers;
using Raven35.Database.Server.WebApi.Attributes;
using Raven35.Json.Linq;

namespace Raven35.Database.TimeSeries.Controllers
{
    public class TimeSeriesController : BaseDatabaseApiController
    {
        [RavenRoute("ts")]
        [HttpGet]
        public HttpResponseMessage TimeSeries(bool getAdditionalData = false)
        {
            return Resources<TimeSeriesData>(Constants.TimeSeries.Prefix, GetTimeSeriesData, getAdditionalData);
        }

        private class TimeSeriesData : TenantData
        {
        }

        private static List<TimeSeriesData> GetTimeSeriesData(IEnumerable<RavenJToken> timeSeries)
        {
            return timeSeries
                .Select(ts =>
                {
                    var bundles = new string[] { };
                    var settings = ts.Value<RavenJObject>("Settings");
                    if (settings != null)
                    {
                        var activeBundles = settings.Value<string>("Raven/ActiveBundles");
                        if (activeBundles != null)
                        {
                            bundles = activeBundles.Split(';');
                        }
                    }
                    return new TimeSeriesData
                    {
                        Name = ts.Value<RavenJObject>("@metadata").Value<string>("@id").Replace(Constants.TimeSeries.Prefix, string.Empty),
                        Disabled = ts.Value<bool>("Disabled"),
                        Bundles = bundles,
                        IsAdminCurrentTenant = true,
                    };
                }).ToList();
        }
    }
}
