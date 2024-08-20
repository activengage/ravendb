extern alias client;
using Raven35.Abstractions.Data;

using Xunit;

using Raven35.Client.Extensions;

namespace Raven35.Tests.Bundles.Authorization.Bugs
{
    public class Kwal : AuthorizationTest
    {
        [Fact]
        public void WillAbortDeleteIfUserDoesNotHavePermissions()
        {
            using (var session = store.OpenSession())
            {
                session.Store(
                    new DatabaseDocument()
                    {
                        Id = "Raven35.Databases/Testing",
                        Settings =
                           {
                               { Constants.RunInMemory, "false" },
                               { "Raven/DataDir", "~\\Testing" }
                           }
                    }
                );

                session.SaveChanges();

                store.DatabaseCommands.EnsureDatabaseExists("Testing");
            }

            using (var session = store.OpenSession("Testing"))
            {
                session.Store(
                    new client::Raven35.Bundles.Authorization.Model.AuthorizationUser()
                    {
                        Id = "Authorization/Users/Johnny",
                        Name = "Johnny Executive"
                    }
                );

                session.SaveChanges();
            }
        }
    }
}
