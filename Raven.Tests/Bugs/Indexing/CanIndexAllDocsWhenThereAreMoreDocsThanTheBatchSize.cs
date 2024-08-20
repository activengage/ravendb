using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.Bugs.Indexing
{
    public class CanIndexAllDocsWhenThereAreMoreDocsThanTheBatchSize : RavenTest
    {
        [Fact]
        public void WillIndexAllWhenCreatingIndex()
        {
            using (var store = NewDocumentStore())
            {
                store.SystemDatabase.Configuration.MaxNumberOfItemsToProcessInSingleBatch = 3;

                using (var session = store.OpenSession())
                {
                    for (int i = 0; i < 15; i++)
                    {
                        session.Store(new User{Name="1"});
                    }
                    session.SaveChanges();
                }

                store.DatabaseCommands.PutIndex("test",
                                                new IndexDefinition
                                                {
                                                    Map = "from doc in docs select new { doc.Name}"
                                                });

                using (var session = store.OpenSession())
                {
                    var users = session.Query<User>("test").Customize(x=>x.WaitForNonStaleResults()).ToArray();

                    Assert.Equal(15, users.Length);
                }
            }
        }

        [Fact]
        public void WillIndexAllAfterCreatingIndex()
        {
            using (var store = NewDocumentStore())
            {
                store.SystemDatabase.Configuration.MaxNumberOfItemsToProcessInSingleBatch = 3;

            
                store.DatabaseCommands.PutIndex("test",
                                                new IndexDefinition
                                                {
                                                    Map = "from doc in docs select new { doc.Name}"
                                                });

                using (var session = store.OpenSession())
                {
                    for (int i = 0; i < 15; i++)
                    {
                        session.Store(new User { Name = "1" });
                    }
                    session.SaveChanges();
                }
                
                using (var session = store.OpenSession())
                {
                    var users = session.Query<User>("test").Customize(x => x.WaitForNonStaleResults()).ToArray();

                    Assert.Equal(15, users.Length);
                }
            }
        }
    }
}
