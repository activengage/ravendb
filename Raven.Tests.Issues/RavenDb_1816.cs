using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDb_1816 : RavenTest
    {
        private const string DbName = "TestDb";
        private const int Quantity = 1000;


        [Fact]
        public void CanLoadLongQuerry()
        {
            List<int> list = Enumerable.Range(1, 3000).ToList();
            using (IDocumentStore store = NewRemoteDocumentStore())
            {
                new LaptopIndex().Execute(store);
                using (IDocumentSession session = store.OpenSession())
                {
                    IRavenQueryable<Laptop> q = session.Query<Laptop, LaptopIndex>()
                        .Where(x => x.Id.In(list));


                    var s = q.ToString();

                    Assert.True(s.Length > (32*1024));

                    using (IEnumerator<StreamResult<Laptop>> streamingQuery = session.Advanced.Stream(q))
                    {
                        Assert.False(streamingQuery.MoveNext());
                    }
                }
            }
        }

        public class LaptopIndex : AbstractIndexCreationTask<Laptop>
        {
            public LaptopIndex()
            {
                Map = laptops => from laptop in laptops
                                 select new
                                 {
                                     laptop.Id
                                 };
            }
        }

        public class Laptop
        {
            public int Id { get; set; }
            public string Cpu { get; set; }
            public string Manufacturer { get; set; }
            public string HDDSizeInGigabytes { get; set; }
            public string RamSizeInMegabatye { get; set; }
        }
    }

    
}
