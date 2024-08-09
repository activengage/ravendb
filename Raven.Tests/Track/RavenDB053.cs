using System;
using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Embedded;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Track
{
    public class JoinedChildTransport
    {
        public string ChildId { get; set; }
        public string TransportId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("ChildId: {0}, TransportId: {1}, Name: {2}", ChildId, TransportId, Name);
        }
    }

    public class Child
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Transport
    {
        public string Id { get; set; }
        public string ChildId { get; set; }
    }

    public class LinqTest : RavenTest
    {
        public class TransportsIndex : AbstractMultiMapIndexCreationTask<JoinedChildTransport>
        {
            public TransportsIndex()
            {
                AddMap<Child>(childList => from child in childList
                                           select new
                                           {
                                            ChildId = child.Id,
                                            TransportId = (dynamic) null, 
                                            child.Name,
                                           });

                AddMap<Transport>(transportList => from transport in transportList
                                                   select new
                                                   {
                                                    transport.ChildId,
                                                    TransportId = transport.Id,
                                                    Name = (dynamic) null,
                                                   });

                Reduce = results => from result in results
                                    group result by result.ChildId
                                    into g
                                        from transport in g.Where(transport => transport.TransportId != null).DefaultIfEmpty()
                                        from child in g.Where(barn=>barn.Name != null).DefaultIfEmpty()
                                    select new { ChildId = g.Key, transport.TransportId, child.Name };

                Store(x => x.ChildId, FieldStorage.Yes);
                Store(x => x.TransportId, FieldStorage.Yes);
                Store(x => x.Name, FieldStorage.Yes);
            }
        }

        [Fact]
        public void ChildrenHasMultipleTransports_Raven()
        {
            using (var docStore = NewDocumentStore())
            {
                // Create Index
                new TransportsIndex().Execute(docStore);

                using (var session = docStore.OpenSession())
                {

                    // Store two children
                    session.Store(new Child {Id = "B1", Name = "Thor Arne"});
                    session.Store(new Child {Id = "B2", Name = "St�le"});

                    // Store four Transports
                    session.Store(new Transport {Id = "A1", ChildId = "B1"});
                    session.Store(new Transport {Id = "A2", ChildId = "B1"});
                    session.Store(new Transport {Id = "A3", ChildId = "B2"});
                    session.Store(new Transport {Id = "A4", ChildId = "B2"});

                    session.SaveChanges();

                    var transports = session.Query<JoinedChildTransport, TransportsIndex>()
                        .Customize(x=>x.WaitForNonStaleResults(TimeSpan.FromMinutes(100)))
                        .OrderBy(x=>x.TransportId)
                        .OrderBy(x=>x.ChildId)
                        .ProjectFromIndexFieldsInto<JoinedChildTransport>()
                        .ToList();

                    Assert.Empty(docStore.SystemDatabase.Statistics.Errors);

                    Assert.Equal(4, transports.Count);

                    // skyssavtaler for B1
                    Assert.Equal("A1", transports[0].TransportId);
                    Assert.Equal("B1", transports[0].ChildId);
                    Assert.Equal("Thor Arne", transports[0].Name);

                    Assert.Equal("A2", transports[1].TransportId);
                    Assert.Equal("B1", transports[1].ChildId);
                    Assert.Equal("Thor Arne", transports[0].Name);

                    // skyssavtaler for B2
                    Assert.Equal("A3", transports[2].TransportId);
                    Assert.Equal("B2", transports[2].ChildId);
                    Assert.Equal("St�le", transports[2].Name);

                    Assert.Equal("A4", transports[3].TransportId);
                    Assert.Equal("B2", transports[3].ChildId);
                    Assert.Equal("St�le", transports[3].Name);
                }
            }
        }
    }
}
