using System.Threading.Tasks;
using Raven35.Tests.Bundles.Replication;
using Raven35.Tests.Common;
using Xunit.Extensions;

namespace Raven35.Tests.Issues
{
    public class RavenDB_4453 : RavenTest
    {
        [Theory]
        [PropertyData("Storages")]
        public void NextIdentityForIsThreadSafe(string requestedStorage)
        {
            using (var documentStore = NewDocumentStore(requestedStorage: requestedStorage))
            {
                Parallel.For(1, 20, i => documentStore.DatabaseCommands.NextIdentityFor("aa"));
            }
        }
    }
}
