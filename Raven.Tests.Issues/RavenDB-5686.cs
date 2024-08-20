using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Raven35.Abstractions.Util.MiniMetrics;
using Raven35.Client.Connection;
using Raven35.Client.Document;
using Raven35.Client.Linq;
using Xunit;
using Raven35.Client.Indexes;
using Raven35.Json.Linq;
using Raven35.Tests.Helpers;

namespace Raven35.Tests.Issues
{
    public class RavenDB_5686
    {
        [Fact]
        public void CanSerializeAndDeserializeMeterValue()
        {
            var meter = new MeterValue(1, 2.0, 3.0, 4.0, 15.0);
            var jObject = RavenJObject.FromObject(meter);
            var parsed = jObject.Deserialize<MeterValue>(new DocumentConvention());

            Assert.Equal(meter.Count, parsed.Count);
            Assert.Equal(meter.MeanRate, parsed.MeanRate);

            Assert.Equal(meter.OneMinuteRate, parsed.OneMinuteRate);
            Assert.Equal(meter.FiveMinuteRate, parsed.FiveMinuteRate);
            Assert.Equal(meter.FifteenMinuteRate, parsed.FifteenMinuteRate);
        }
    }
}