using Raven35.Abstractions.TimeSeries;
using Raven35.Database.Config;
using Raven35.Database.TimeSeries;

namespace Raven35.Tests.TimeSeries
{
    public class TimeSeriesTest
    {
        public static TimeSeriesStorage GetStorage()
        {
            var storage = new TimeSeriesStorage("http://localhost:8080/", "TimeSeriesTest", new RavenConfiguration { RunInMemory = true });
            using (var writer = storage.CreateWriter())
            {
                writer.CreateType("Simple", new[] {"Value"});
                writer.Commit();
            }
            return storage;
        }
    }
}