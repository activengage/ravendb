//-----------------------------------------------------------------------
// <copyright file="IndexWithTwoProperties.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class IndexWithTwoProperties : RavenTest
    {
        [Fact]
        public void CanCreateIndexByTwoProperties()
        {
            using (var store = NewDocumentStore())
            using (var session = store.OpenSession())
            {
                session.Store(new Foo { Id = "1", Value = "foo" });


                session.Store(new Foo { Id = "2", Value = "bar" });


                session.SaveChanges();

                store.DatabaseCommands.PutIndex(
                    "FeedSync/TwoProperties",
                    new IndexDefinitionBuilder<Foo>


                    {
                        Map = ids => from id in ids
                                        select new { id.Id, id.Value },
                    },
                    true);
            }
        }

        public class Foo 
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }
    }
}
