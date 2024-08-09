using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class Distinct : RavenTest
    {
        [Fact]
        public void CanQueryForDistinctItems()
        {
            using (var store = NewDocumentStore())
            {
                using (var s = store.OpenSession())
                {
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "rahien" });
                    s.SaveChanges();
                }

                store.SystemDatabase.Indexes.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.Name }",
                    Stores = { { "Name", FieldStorage.Yes } }
                });

                using (var s = store.OpenSession())
                {
                    var objects = s.Advanced.DocumentQuery<dynamic>("test")
                        .WaitForNonStaleResults()
                        .SelectFields<dynamic>("Name")
                        .OrderBy("Name")
                        .Distinct()
                        .OrderBy("Name")
                        .ToList();

                    Assert.Equal(2, objects.Count);
                    Assert.Equal("ayende", objects[0].Name);
                    Assert.Equal("rahien", objects[1].Name);
                }
            }
        }

        [Fact]
        public void CanQueryForDistinctItemsUsingLinq()
        {
            using (var store = NewDocumentStore())
            {
                using (var s = store.OpenSession())
                {
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "rahien" });
                    s.SaveChanges();
                }

                store.SystemDatabase.Indexes.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.Name }",
                    Stores = { { "Name", FieldStorage.Yes } }
                });

                using (var s = store.OpenSession())
                {
                    var objects = s.Query<User>("test")
                        .Customize(x => x.WaitForNonStaleResults())
                        .Select(o => new {o.Name }).OrderBy(o => o.Name)
                        .Select(o => new { o.Name })
                        .Distinct()
                        .ToList();

                    Assert.Equal(2, objects.Count);
                    Assert.Equal("ayende", objects[0].Name);
                    Assert.Equal("rahien", objects[1].Name);
                }
            }
        }
        [Fact]
        public void CanQueryForDistinctItemsUsingLinq_WithPaging()
        {
            using (var store = NewDocumentStore())
            {
                using (var s = store.OpenSession())
                {
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "rahien" });
                    s.SaveChanges();
                }

                store.SystemDatabase.Indexes.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.Name }",
                    Stores = { { "Name", FieldStorage.Yes } }
                });

                using (var s = store.OpenSession())
                {
                    var objects = s.Query<User>("test")
                        .Customize(x => x.WaitForNonStaleResults())
                        .OrderBy(x => x.Name)
                        .Select(o => new { o.Name })
                        .Distinct()
                        .OrderBy(o => o.Name)
                        .Skip(1)
                        .ToList();

                    Assert.Equal(1, objects.Count);
                    Assert.Equal("rahien", objects[0].Name);
                }
            }
        }
        [Fact]
        public void CanQueryForDistinctItemsAndProperlyPage()
        {
            using (var store = NewDocumentStore())
            {
                using (var s = store.OpenSession())
                {
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "ayende" });
                    s.Store(new { Name = "rahien" });
                    s.SaveChanges();
                }

                store.SystemDatabase.Indexes.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.Name }",
                    Stores = { { "Name", FieldStorage.Yes } }
                });

                using (var s = store.OpenSession())
                {
                    var objects = s.Advanced.DocumentQuery<dynamic>("test")
                        .WaitForNonStaleResults()
                        .OrderBy("Name")
                        .Skip(1)
                        .OrderBy("Name")
                        .SelectFields<dynamic>("Name")
                        .Distinct()
                        .ToList();

                    Assert.Equal(1, objects.Count);
                    Assert.Equal("rahien", objects[0].Name);
                }
            }
        }

        [Fact]
        public void IncludeWithDistinct()
        {
            using (var store = NewDocumentStore())
            {
                new CustomersIndex().Execute(store);

                using (var session = store.OpenSession())
                {
                    session.Store(new Customer
                    {
                        Location = "PT CT",
                        Occupation = "Marketing",
                        CustomerId = "1",
                        HeadingId = "2"
                    }, "Customers/2");
                    session.SaveChanges();
                }

                WaitForIndexing(store);
                using (var session = store.OpenSession())
                {

                    RavenQueryStatistics qs;
                    var qRes = session.Advanced.DocumentQuery<Customer>("CustomersIndex")
                        .Statistics(out qs).Where("Occupation:Marketing")
                        .Distinct()
                        .SelectFields<Customer>("CustomerId")
                        .Take(20)
                        .Include("__document_id")
                        .ToList();
                    var cust = session.Load<Customer>("Customers/2");
                    Assert.Equal(1, session.Advanced.NumberOfRequests);
                }
            }
        }

        [Fact]
        public void DistinctWithMapReduce()
        {
            using (var store = NewDocumentStore())
            {
                new ReducedCustomersIndex().Execute(store);

                using (var session = store.OpenSession())
                {
                    session.Store(new Customer
                    {
                        Location = "PT CT",
                        Occupation = "Marketing",
                        CustomerId = "1",
                        HeadingId = "2"
                    }, "Customers/2");
                    session.SaveChanges();
                }

                WaitForIndexing(store);
                using (var session = store.OpenSession())
                {

                    RavenQueryStatistics qs;
                    var qRes = session.Advanced.DocumentQuery<Customer>("ReducedCustomersIndex")
                        .Statistics(out qs).Where("Occupation:Marketing")
                        .Distinct()
                        .SelectFields<Customer>("CustomerId")
                        .Take(20)
                        .ToList();
                    Assert.Equal(1, qs.TotalResults);
                }
            }
        }

        public class Customer
        {
            public string Location { get; set; }
            public string Occupation { get; set; }
            public string CustomerId { get; set; }
            public string HeadingId { get; set; }
        }

        public class CustomersIndex : AbstractIndexCreationTask<Customer>
        {
            public CustomersIndex()
            {
                Map = customers => from customer in customers
                                   select new { customer.Occupation, customer.CustomerId };
                Store(x => x.Occupation, FieldStorage.Yes);
                Store(x => x.CustomerId, FieldStorage.Yes);
            }
        }

        public class ReducedCustomersIndex : AbstractIndexCreationTask<Customer, ReducedCustomersIndex.Result>
        {
            public class Result
            {
                public string Occupation { get; set; }
                public string CustomerId { get; set; }
                public int Count { get; set; }
            }
            public ReducedCustomersIndex()
            {
                Map = customers => from customer in customers
                                   select new { customer.Occupation, customer.CustomerId, Count = 1 };
                Reduce = results => from result in results
                                    group result by new { result.Occupation, result.CustomerId, result.Count }
                                        into g
                                        select new
                                        {
                                            Occupation = g.Key.Occupation,
                                            CustomerId = g.Key.CustomerId,
                                            Count = g.Sum(x => x.Count)
                                        };
                Store(x => x.Occupation, FieldStorage.Yes);
                Store(x => x.CustomerId, FieldStorage.Yes);
            }
        }
    }
}
