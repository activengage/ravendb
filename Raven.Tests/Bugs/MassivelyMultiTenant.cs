using Raven35.Client.Document;
using Raven35.Client.Extensions;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class MassivelyMultiTenant : RavenTest
        {
        [Fact]
        public void CanHaveLotsOf_ACTIVE_Tenants()
        {
            using (GetNewServer(requestedStorage: "esent"))
            {
            for (int i = 0; i < 20; i++)
            {
                var databaseName = "Tenants" + i;
                    using (var documentStore = new DocumentStore {Url = "http://localhost:8079", DefaultDatabase = databaseName}.Initialize())
                {
                    documentStore.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(databaseName);
                }
            }
        }
        }
    }
}
