using System.IO;
using Microsoft.Owin.Builder;
using Owin;
using Raven35.Database.Config;
using Raven35.Database.Server;
using Raven35.Tests.Common;

using Xunit;
using Raven35.Abstractions.Data;

namespace Raven35.Tests
{
    public class AppBuilderExtensionsTests : RavenTest
    {
        [Fact]
        public void When_HostOnAppDisposing_key_not_exist_then_should_not_throw()
        {
            string path = NewDataPath();
            var configuration = new InMemoryRavenConfiguration { Settings =
            {
                { "Raven/DataDir", path },
                { Constants.FileSystem.DataDirectory, Path.Combine(path, "FileSystem")}
            } };

            configuration.Initialize();

            using (var options = new RavenDBOptions(configuration))
            {
                Assert.DoesNotThrow(() => new AppBuilder().UseRavenDB(options));
            }
            
        }
    }
}
