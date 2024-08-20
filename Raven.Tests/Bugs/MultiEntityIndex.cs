//-----------------------------------------------------------------------
// <copyright file="MultiEntityIndex.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;
using Raven35.Tests.Document;
using Xunit;
using System.Linq;

namespace Raven35.Tests.Bugs
{
    public class MultiEntityIndex : RavenTest
    {
        [Fact]
        public void CanCreateIndexOnMultipleEntities()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs.WhereEntityIs(\"Users\", \"Companies\") select new { doc.Name }"
                });

            }
        }

        [Fact]
        public void CanIndexFromMultipleEntitiesUsingWhereEntityIs()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs.WhereEntityIs(\"Users\", \"Companies\") select new { doc.Name }"
                });

                using(var s = store.OpenSession())
                {
                    s.Store(new User{Name="ayende"});
                    s.Store(new Company{Name = "Hibernating Rhinos"});
                    s.SaveChanges();
                }


                using(var s = store.OpenSession())
                {
                    object[] objects = s.Query<object>("test")
                        .Customize(x=>x.WaitForNonStaleResults())
                        .ToArray()
                        .OrderBy(x=>x.GetType().Name)
                        .ToArray();

                    Assert.IsType<Company>(objects[0]);
                    Assert.IsType<User>(objects[1]);
                }
            }
        }
    }
}
