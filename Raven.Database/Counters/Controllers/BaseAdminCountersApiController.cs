// -----------------------------------------------------------------------
//  <copyright file="BaseCountersApiController.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Common;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.Counters.Controllers
{
    public abstract class BaseAdminCountersApiController : AdminResourceApiController<CounterStorage, CountersLandlord>
    {
        public override ResourceType ResourceType
        {
            get
            {
                return ResourceType.Counter;
            }
        }

        public string CounterName
        {
            get { return ResourceName; }
        }

        public CounterStorage Counters
        {
            get { return Resource; }
        }

        public override void MarkRequestDuration(long duration)
        {
            if (Counters == null)
                return;
            Counters.MetricsCounters.RequestDurationMetric.Update(duration);
        }
    }
}
