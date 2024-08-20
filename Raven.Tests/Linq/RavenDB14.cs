using System.Collections.Generic;
using Raven35.Client;
using Raven35.Client.Listeners;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;
using Raven35.Client.Linq;
using System.Linq;

namespace Raven35.Tests.Linq
{
    public class RavenDB14 : RavenTest
    {
        public List<string> Queries = new List<string>();

        [Fact]
        public void WhereThenFirstHasAND()
        {
            using(var store = NewDocumentStore())
            {
                store.RegisterListener(new RecordQueriesListener(Queries));
                var documentSession = store.OpenSession();
                var _ = documentSession.Query<User>().Where(x=>x.Name == "ayende").FirstOrDefault(x=>x.Active);

                Assert.Equal("Name:ayende AND Active:true", Queries[0]);
            }
        }

        [Fact]
        public void WhereThenSingleHasAND()
        {
            using (var store = NewDocumentStore())
            {
                store.RegisterListener(new RecordQueriesListener(Queries));
                var documentSession = store.OpenSession();
                var _ = documentSession.Query<User>().Where(x => x.Name == "ayende").SingleOrDefault(x => x.Active);

                Assert.Equal("Name:ayende AND Active:true", Queries[0]);
            }
        }
    }

    public class RecordQueriesListener : IDocumentQueryListener
    {
        private readonly List<string> queries;

        public RecordQueriesListener(List<string> queries)
        {
            this.queries = queries;
        }

        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queries.Add(queryCustomization.ToString());
        }
    }
}
