using Raven35.Abstractions.Replication;

namespace Raven35.Client.Counters
{
    /// <summary>
    /// The set of conventions used by the <see cref="CountersConvention"/> which allow the users to customize
    /// the way the Raven35.Client API behaves
    /// </summary>
    public class CountersConvention: ConventionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountersConvention"/> class.
        /// </summary>
        public CountersConvention()
        {
            FailoverBehavior = FailoverBehavior.AllowReadsFromSecondaries;
            AllowMultipuleAsyncOperations = true;
            ShouldCacheRequest = url => true;
        }
    }
}
