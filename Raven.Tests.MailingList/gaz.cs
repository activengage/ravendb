// -----------------------------------------------------------------------
//  <copyright file="gaz.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Extensions;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class gaz : RavenTest
    {
        public class Foo
        {
            public string Id { get; set; }
            public int CatId { get; set; }
            public double Lat { get; set; }
            public double Long { get; set; }
        }

        public class Foos : AbstractIndexCreationTask<Foo>
        {
            public Foos()
            {
                Map = foos => from foo in foos
                              select new { foo.Id, foo.CatId, _ = SpatialGenerate("Position", foo.Lat, foo.Long) };
            }
        }

        [Fact]
        public void SpatialSearchBug2()
        {
            using (var store = NewRemoteDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    var foo = new Foo() { Lat = 20, Long = 20, CatId = 1 };
                    session.Store(foo);
                    session.SaveChanges();

                    new Foos().Execute(store);

                    WaitForIndexing(store);

                    var query2 = session.Advanced.LuceneQuery<Foo, Foos>()
                        .UsingDefaultOperator(QueryOperator.And)
                        .WithinRadiusOf("Position", 100, 20, 20, SpatialUnits.Miles)
                        .Where("CatId:2")
                        .Where("CatId:1")
                        .ToList();

                    Assert.Empty(query2);
                }
            }
        }
    }
}
