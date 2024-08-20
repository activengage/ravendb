// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3442.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;

using Raven35.Abstractions.Indexing;
using Raven35.Client.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3442 : RavenTest
    {
        [Fact]
        public void WhereEqualsShouldSendSortHintsAndDynamicIndexesShouldSetAppropriateSortOptionsThen1()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Query<User>()
                        .Where(x => x.Age == 10)
                        .ToList();
                }

                var indexes = store.DatabaseCommands.GetIndexes(0, 10);
                var index = indexes.Single(x => x.Name.StartsWith("Auto/"));

                Assert.Equal(1, index.SortOptions.Count);
                Assert.Equal(SortOptions.Int, index.SortOptions["Age"]);
            }
        }

    }
}
