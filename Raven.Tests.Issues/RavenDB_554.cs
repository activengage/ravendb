using Raven35.Tests.Common;

namespace Raven35.Tests.Issues
{
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using Raven35.Abstractions.Data;
    using Raven35.Abstractions.Extensions;
    using Raven35.Abstractions.Indexing;
    using Raven35.Client.Document;

    using Xunit;

    public class RavenDB_554 : RavenTest
    {
        public class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string MiddleName { get; set; }
        }

        [Fact]
        public void IndexEntryFieldShouldNotContainNullValues()
        {
            const string IndexName = "Index1";

            using (var server = GetNewServer())
            {
                server.SystemDatabase.Indexes.PutIndex(IndexName, new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.FirstName, doc.LastName, Query = new[] { doc.FirstName, doc.LastName, doc.MiddleName } }",
                    Indexes =
                    {
                        { "Query", FieldIndexing.Analyzed }
                    }
                });

                using (var docStore = new DocumentStore { Url = "http://localhost:8079" }.Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        session.Store(new Person { FirstName = "John", MiddleName = null, LastName = null });
                        session.Store(new Person { FirstName = "William", MiddleName = "Edgard", LastName = "Smith" });
                        session.Store(new Person { FirstName = "Paul", MiddleName = null, LastName = "Smith" });
                        session.SaveChanges();
                    }

                    using (var session = docStore.OpenSession())
                    {
                        session.Query<Person>(IndexName)
                            .Customize(x => x.WaitForNonStaleResults())
                            .ToList();

                        var queryResult = session.Advanced.DocumentStore.DatabaseCommands.Query(IndexName, new IndexQuery(), null, false, true);
                        foreach (var result in queryResult.Results)
                        {
                            var q = result["Query"].ToString();
                            Assert.NotNull(q);
                            Assert.False(q.Contains(Constants.NullValue));
                        }
                    }
                }
            }
        }
    }
}
