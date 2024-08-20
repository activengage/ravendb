using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.Bugs.Indexing
{
    public class FilterOnMissingProperty : RavenTest
    {
        [Fact]
        public void CanFilter()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test",
                                                new IndexDefinition
                                                    {
                                                        Map = "from doc in docs where doc.Valid select new { doc.Name }"
                                                    });

                using(var session = store.OpenSession())
                {
                    session.Store(new { Valid = true, Name = "Oren"});

                    session.Store(new { Name = "Ayende "});

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    session.Advanced.DocumentQuery<dynamic>("test").WaitForNonStaleResults().ToArray();
                }

                Assert.Empty(store.SystemDatabase.Statistics.Errors);
            }
        }
    }
}
