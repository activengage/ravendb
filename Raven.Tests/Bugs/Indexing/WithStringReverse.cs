using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Linq;
using Raven35.Client.Embedded;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class WithStringReverse : RavenTest
    {
        [Fact]
        public void GivenSomeUsers_QueryWithAnIndex_ReturnsUsersWithNamesReversed()
        {
            using (EmbeddableDocumentStore store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("StringReverseIndex",
                                                new IndexDefinition
                                                    {
                                                        Map =
                                                            "from doc in docs select new { doc.Name, ReverseName = doc.Name.Reverse()}"
                                                    });

                using (IDocumentSession documentSession = store.OpenSession())
                {
                    documentSession.Store(new User {Name = "Ayende"});
                    documentSession.Store(new User {Name = "Itamar"});
                    documentSession.Store(new User {Name = "Pure Krome"});
                    documentSession.Store(new User {Name = "John Skeet"});
                    documentSession.Store(new User {Name = "StackOverflow"});
                    documentSession.Store(new User {Name = "Wow"});
                    documentSession.SaveChanges();
                }

                using (IDocumentSession documentSession = store.OpenSession())
                {
                    var users = documentSession
                        .Query<User>("StringReverseIndex")
                        .Customize(x=>x.WaitForNonStaleResults())
                        .ToList();

                    Assert.Empty(store.SystemDatabase.Statistics.Errors);
                    Assert.NotNull(users);
                    Assert.True(users.Count > 0);
                }
            }
        }

        public class ReversedResult
        {
            public string ReverseName { get; set; }
        }

        protected override void CreateDefaultIndexes(IDocumentStore documentStore)
        {
        }

        [Fact]
        public void CanQueryInReverse()
        {
            using (EmbeddableDocumentStore store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("StringReverseIndex",
                                                new IndexDefinition
                                                {
                                                    Map =
                                                        "from doc in docs select new { doc.Name, ReverseName = doc.Name.Reverse()}"
                                                });

                using (IDocumentSession documentSession = store.OpenSession())
                {
                    documentSession.Store(new User { Name = "Ayende" });
                    documentSession.Store(new User { Name = "Itamar" });
                    documentSession.Store(new User { Name = "Pure Krome" });
                    documentSession.Store(new User { Name = "John Skeet" });
                    documentSession.Store(new User { Name = "StackOverflow" });
                    documentSession.Store(new User { Name = "Wow" });
                    documentSession.SaveChanges();
                }

                using (IDocumentSession documentSession = store.OpenSession())
                {
                    var users = documentSession
                        .Query<ReversedResult>("StringReverseIndex")
                        .Customize(x => x.WaitForNonStaleResults())
                        .Where(x=>x.ReverseName.StartsWith("edn"))
                        .As<User>()
                        .ToList();

                    Assert.Empty(store.SystemDatabase.Statistics.Errors);
                    Assert.True(users.Count > 0);
                }
            }
        }
    }
}
