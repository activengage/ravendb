// -----------------------------------------------------------------------
//  <copyright file="RavenDB_659.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;

namespace Raven35.Tests.Issues
{
    using System.Linq;
    using System.Threading;

    using Raven35.Client;
    using Raven35.Client.Indexes;
    using Raven35.Json.Linq;

    using Xunit;

    public class RavenDB_659 : RavenTest
    {
        private class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private class Index : AbstractIndexCreationTask<User>
        {
            public Index()
            {
                Map = users => from u in users select new
                                                      {
                                                          Name = u.Name
                                                      };
            }
        }

        private void AssertNumberOfDocuments(IDocumentStore store, int expectedCount)
        {
            using (var session = store.OpenSession())
            {
                var currentCount = session.Query<User>().Customize(x => x.WaitForNonStaleResults()).Count();

                Assert.Equal(expectedCount, currentCount);
            }
        }

        [Fact]
        public void IndexingShouldNotIndexDeletedDocuments()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Id = 1 });
                    session.Store(new User { Id = 2 });
                    session.SaveChanges();
                }

                AssertNumberOfDocuments(store, 2);

                store.SystemDatabase.StopBackgroundWorkers();

                using (var session = store.OpenSession())
                {
                    session.Store(new User { Id = 3 });
                    session.Store(new User { Id = 4 });
                    session.SaveChanges();
                }

                store.DatabaseCommands.Delete("users/1", null);
                store.DatabaseCommands.Delete("users/4", null);

                store.SystemDatabase.SpinBackgroundWorkers();

                AssertNumberOfDocuments(store, 2);

                using (var session = store.OpenSession())
                {
                    session.Store(new User { Id = 5 });
                    session.Store(new User { Id = 6 });
                    session.SaveChanges();
                }

                AssertNumberOfDocuments(store, 4);
            }
        }
    }
}
