// -----------------------------------------------------------------------
//  <copyright file="QueryResultsStreaming.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Core.Utils.Entities;
using Raven35.Tests.Core.Utils.Indexes;
using Xunit;

namespace Raven35.Tests.Core.Streaming
{
    public class QueryStreaming : RavenCoreTestBase
    {
#if DNXCORE50
        public QueryStreaming(TestServerFixture fixture)
            : base(fixture)
        {

        }
#endif

        [Fact]
        public void CanStreamQueryResults()
        {
            using (var store = GetDocumentStore())
            {
                new Users_ByName().Execute(store);

                using (var session = store.OpenSession())
                {
                    for (int i = 0; i < 200; i++)
                    {
                        session.Store(new User());
                    }
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                int count = 0;

                using (var session = store.OpenSession())
                {
                    var query = session.Query<User, Users_ByName>();

                    var reader = session.Advanced.Stream(query);

                    while (reader.MoveNext())
                    {
                        count++;
                        Assert.IsType<User>(reader.Current.Document);

                    }
                }

                count = 0;

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced.DocumentQuery<User, Users_ByName>();
                    var reader = session.Advanced.Stream(query);
                    while (reader.MoveNext())
                    {
                        count++;
                        Assert.IsType<User>(reader.Current.Document);

                    }
                }

                Assert.Equal(200, count);
            }
        }
    }
}
