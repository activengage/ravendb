#if !DNXCORE50
using System;

using Raven35.Abstractions.Util;
using Raven35.Database.Util;
using Xunit;

namespace Raven35.Tests.Core
{
    public class RavenGCTests : IDisposable
    {
        public RavenGCTests()
        {
            RavenGC.ResetHistory();
        }

        [Fact]
        public void Before_and_after_memory_allocation_should_be_recorded_correctly()
        {
            WeakReference reference;
            new Action(() =>
            {
                var bytes = new byte[4096 * 1024];
                new Random().NextBytes(bytes);
                reference = new WeakReference(bytes, true);
            })();

            RavenGC.CollectGarbage(true, () => { }, true);

            Assert.True(RavenGC.MemoryBeforeLastForcedGC > RavenGC.MemoryAfterLastForcedGC);
        }

        public void Dispose()
        {
        }
    }
}
#endif