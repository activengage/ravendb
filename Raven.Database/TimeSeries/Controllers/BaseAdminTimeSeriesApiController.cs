// -----------------------------------------------------------------------
//  <copyright file="BaseAdminTimeSeriesApiController.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Common;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.TimeSeries.Controllers
{
    public class BaseAdminTimeSeriesApiController : AdminResourceApiController<TimeSeriesStorage, TimeSeriesLandlord>
    {
        public string TimeSeriesName
        {
            get { return ResourceName; }
        }

        public TimeSeriesStorage TimeSeries
        {
            get { return Resource; }
        }

        public override ResourceType ResourceType
        {
            get
            {
                return ResourceType.TimeSeries;
            }
        }

        public override void MarkRequestDuration(long duration)
        {
            if (TimeSeries == null)
                return;
            TimeSeries.MetricsTimeSeries.RequestDurationMetric.Update(duration);
        }
    }
}
