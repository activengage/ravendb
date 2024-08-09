// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2440.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;

using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2440 : RavenTest
    {
        [Fact]
        public void TimingsShouldNotBeCalculatedByDefault()
        {
            using (var store = NewDocumentStore())
            {
                new RavenDocumentsByEntityName().Execute(store);

                using (var session = store.OpenSession())
                {
                    session.Store(new Person());
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                var result = store
                    .DatabaseCommands
                    .Query(Constants.DocumentsByEntityNameIndex, new IndexQuery());

                Assert.Equal(0, result.TimingsInMilliseconds.Count);

                using (var session = store.OpenSession())
                {
                    RavenQueryStatistics queryStats;
                    var results = session.Query<object>(Constants.DocumentsByEntityNameIndex)
                        .Customize(x => x.NoCaching())
                        .Statistics(out queryStats)
                        .ToList();

                    Assert.Equal(0, queryStats.TimingsInMilliseconds.Count);

                    session.Query<User>()
                        .Customize(x=>x.ShowTimings())
                        .Statistics(out queryStats)
                        .Where(x => x.Age > 15)
                        .ToList();


                    RavenQueryStatistics documentQueryStats;
                    results = session.Advanced.DocumentQuery<object>(Constants.DocumentsByEntityNameIndex)
                        .NoCaching()
                        .Statistics(out documentQueryStats)
                        .ToList();

                    Assert.Equal(0, documentQueryStats.TimingsInMilliseconds.Count);
                }
            }
        }

        [Fact]
        public void TimingsShouldBeCalculatedIfRequested()
        {
            using (var store = NewDocumentStore())
            {
                new RavenDocumentsByEntityName().Execute(store);

                using (var session = store.OpenSession())
                {
                    session.Store(new Person());
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                var result = store
                    .DatabaseCommands
                    .Query(Constants.DocumentsByEntityNameIndex, new IndexQuery { ShowTimings = true });

                Assert.NotEmpty(result.TimingsInMilliseconds);

                using (var session = store.OpenSession())
                {
                    RavenQueryStatistics queryStats;
                    var results = session.Query<object>(Constants.DocumentsByEntityNameIndex)
                        .Customize(x =>
                        {
                            x.ShowTimings();
                            x.NoCaching();
                        })
                        .Statistics(out queryStats)
                        .ToList();

                    Assert.NotEmpty(result.TimingsInMilliseconds);

                    RavenQueryStatistics documentQueryStats;
                    results = session.Advanced.DocumentQuery<object>(Constants.DocumentsByEntityNameIndex)
                        .ShowTimings()
                        .NoCaching()
                        .Statistics(out documentQueryStats)
                        .ToList();

                    Assert.NotEmpty(result.TimingsInMilliseconds);

                    Assert.NotEqual(0, documentQueryStats.ResultSize);
                }
            }
        }
    }
}
