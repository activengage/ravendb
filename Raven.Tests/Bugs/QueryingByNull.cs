//-----------------------------------------------------------------------
// <copyright file="QueryingByNull.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class QueryingByNull : RavenTest
    {
        [Fact]
        public void CanQueryByNullUsingLinq()
        {
            using(var store = NewDocumentStore())
            {
                using(var session = store.OpenSession())
                {
                    session.Store(new Person());
                    session.SaveChanges();
                }

                store.DatabaseCommands.PutIndex("People/ByName",
                                                new IndexDefinition
                                                {
                                                    Map = "from doc in docs.People select new { doc.Name}"
                                                });

                using(var session = store.OpenSession())
                {
                    var q = from person in session.Query<Person>("People/ByName")
                                .Customize(x=>x.WaitForNonStaleResults())
                            where person.Name == null
                            select person;
                    Assert.Equal(1, q.Count());
                }
            }
        }

        public class Person
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
