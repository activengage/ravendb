//-----------------------------------------------------------------------
// <copyright file="IndexingRavenDocuments.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class IndexingRavenDocuments : RavenTest
    {
        [Fact]
        public void WillNotIndexRavenDocuments()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("Users",
                                                new IndexDefinition
                                                {
                                                    Map = "from user in docs select new { user.Name}"
                                                });

                using(var s = store.OpenSession())
                {
                    s.Store(new User{Name = "Ayende"});

                    s.SaveChanges();
                }

                using (var s = store.OpenSession())
                {
                    var waitForNonStaleResults = s.Advanced.DocumentQuery<User>("Users")
                        .WaitForNonStaleResults();
                    Assert.Equal(1, waitForNonStaleResults.QueryResult.TotalResults);
                }
            }

        }

        public class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
