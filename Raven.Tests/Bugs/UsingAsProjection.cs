using System.Collections.Generic;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Database.Indexing;
using System.Linq;

using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class UsingAsProjection : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            using(GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = "http://localhost:8079" })
                {
                    documentStore.Initialize();

                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var company = new Account
                        {
                            Name = "Account A",
                        };
                        company.Users.Add(new User { Email = "a", Password = "1" });
                        company.Users.Add(new User { Email = "b", Password = "2" });
                        session.Store(company);

                        company = new Account
                        {
                            Name = "Account B",
                        };
                        company.Users.Add(new User { Email = "c", Password = "3" });
                        session.Store(company);

                        session.SaveChanges();
                    }
                }

                using (var documentStore = new DocumentStore { Url = "http://localhost:8079" })
                {
                    documentStore.Initialize();
                    new UsersIndexTask().Execute(documentStore);
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var user = session.Query<Account, UsersIndexTask>()
                            .Customize(x=>x.WaitForNonStaleResults())
                            .ProjectFromIndexFieldsInto<User>()
                            .Where(x => x.Email == "a")
                            .FirstOrDefault();
                        Assert.Equal("1", user.Password);
                    }
                }
            }
        }

        public class UsersIndexTask : AbstractIndexCreationTask<Account, User>
        {
            public UsersIndexTask()
            {
                Map = accounts => from account in accounts
                                  from user in account.Users
                                  select new { user.Email, user.Password };

                Stores.Add(x => x.Email, FieldStorage.Yes);
                Stores.Add(x => x.Password, FieldStorage.Yes);
            }
        }

        public class Account
        {
            public Account()
            {
                Users = new List<User>();
            }
            public List<User> Users { get; private set; }
            public string Name { get; set; }
        }

        public class User
        {
            public string Password { get; set; }
            public string Email { get; set; }
        }
    }
}
