// -----------------------------------------------------------------------
//  <copyright file="BaseCountersApiController.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Common;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.Counters.Controllers
{
    public abstract class BaseCountersApiController : ResourceApiController<CounterStorage, CountersLandlord>
    {
        protected string CountersName => ResourceName;

        protected CounterStorage CounterStorage => Resource;

        public override ResourceType ResourceType => ResourceType.Counter;

        public override void MarkRequestDuration(long duration)
        {
            Resource?.MetricsCounters.RequestDurationMetric.Update(duration);
        }
    }
}
