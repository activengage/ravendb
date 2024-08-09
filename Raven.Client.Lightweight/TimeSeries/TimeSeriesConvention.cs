using System;
using System.Net.Http;
using System.Threading.Tasks;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Replication;

namespace Raven35.Client.TimeSeries
{
    /// <summary>
    /// The set of conventions used by the <see cref="TimeSeriesConvention"/> which allow the users to customize
    /// the way the Raven35.Client API behaves
    /// </summary>
    public class TimeSeriesConvention : ConventionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesConvention"/> class.
        /// </summary>
        public TimeSeriesConvention()
        {
            FailoverBehavior = FailoverBehavior.AllowReadsFromSecondaries;
            AllowMultipuleAsyncOperations = true;
            ShouldCacheRequest = url => true;
        }
    }
}
