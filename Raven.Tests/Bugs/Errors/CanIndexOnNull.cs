using System;
using Raven35.Abstractions.Indexing;
using Raven35.Json.Linq;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.Bugs.Errors
{
    public class CanIndexOnNull : RavenTest
    {
        [Fact]
        public void CanIndexOnMissingProps()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test",
                                                new IndexDefinition
                                                {
                                                    Map = "from doc in docs select new { doc.Type, doc.Houses.Wheels} "
                                                });

                for (int i = 0; i < 50; i++)
                {
                    store.DatabaseCommands.Put("item/" + i, null,
                                               new RavenJObject {{"Type", "Car"}}, new RavenJObject());
                }


                using(var s = store.OpenSession())
                {
                    s.Advanced.DocumentQuery<dynamic>("test")
                        .WaitForNonStaleResults()
                        .WhereGreaterThan("Wheels_Range", 4)
                        .ToArray();
                    
                }

                Assert.Empty(store.SystemDatabase.Statistics.Errors);
            }
        }
    }
}
