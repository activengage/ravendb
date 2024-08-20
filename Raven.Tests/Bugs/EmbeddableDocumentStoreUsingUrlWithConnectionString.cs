using Raven35.Client.Connection;
using Raven35.Client.Embedded;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class EmbeddableDocumentStoreUsingUrlWithConnectionString : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            using (var store = new EmbeddableDocumentStore
            {
                ConnectionStringName = "Server"
            })
            {
                store.Initialize();
                Assert.IsType<ServerClient>(store.DatabaseCommands);
                Assert.Null(store.SystemDatabase);
            }
        }
    }
}
