using System.Collections.Generic;

namespace Raven35.Abstractions.TimeSeries
{
    public class TimeSeriesReplicationStats
    {
        public List<TimeSeriesDestinationStats> Stats { get; set; }
    }
}
