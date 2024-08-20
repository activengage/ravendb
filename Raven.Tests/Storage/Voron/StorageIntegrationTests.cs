using Raven35.Tests.Common;

namespace Raven35.Tests.Storage.Voron
{
    using Xunit;

    [Trait("VoronTest", "StorageActionsTests")]
    public class StorageIntegrationTests : RavenTest
    {
        [Fact]
        public void RemoteStorageInitialized_Exception_Not_Thrown()
        {
            Assert.DoesNotThrow(() =>
            {
                using (var ravenDbServer = GetNewServer(requestedStorage: "voron", runInMemory: false))
                {
                    using (NewRemoteDocumentStore(requestedStorage: "voron", runInMemory: false, ravenDbServer: ravenDbServer))
                    {
                    }

                    Assert.Equal(ravenDbServer.SystemDatabase.TransactionalStorage.FriendlyName, "Voron");
                }
            });
        }
    }
}
