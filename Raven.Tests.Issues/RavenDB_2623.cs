// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2623.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

using Raven35.Abstractions.Data;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2623 : RavenTest
    {
        [Fact]
        public void WaitForCompletionShouldThrowWhenOperationHasFaulted()
        {
            using (var store = NewRemoteDocumentStore())
            {
                store
                    .DatabaseCommands
                    .Admin
                    .StopIndexing();

                using (var session = store.OpenSession())
                {
                    for (var i = 0; i < 100; i++)
                    {
                        session.Store(new Person { Name = Guid.NewGuid().ToString() });
                    }

                    session.SaveChanges();
                }

                var e = Assert.Throws<InvalidOperationException>(() => store
                    .DatabaseCommands
                    .DeleteByIndex(Constants.DocumentsByEntityNameIndex, new IndexQuery { Query = "Tag:People" }, options: null)
                    .WaitForCompletion());

                Assert.Equal("Operation failed: Bulk operation cancelled because the index is stale and allowStale is false", e.Message);
            }
        }
    }
}
