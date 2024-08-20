using System.Transactions;
using Raven35.Abstractions.Commands;
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Server;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class PatchingWithDTC : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            using (RavenDbServer server = GetNewServer(requestedStorage: "esent"))
            {
                EnsureDtcIsSupported(server);

                using (IDocumentStore store = new DocumentStore
                {
                    Url = "http://localhost:8079"
                }.Initialize())
                {
                    using (IDocumentSession session = store.OpenSession())
                    {
                        session.Store(new Item {Name = "milk"});
                        session.SaveChanges();
                    }

                    using (var tx = new TransactionScope())
                    using (IDocumentSession session = store.OpenSession())
                    {
                        session.Advanced.Defer(new PatchCommandData
                        {
                            Key = "items/1",
                            Patches = new[]
                            {
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Name",
                                    Value = "Bread"
                                }
                            }
                        });
                        session.SaveChanges();
                        tx.Complete();
                    }
                    using (IDocumentSession session = store.OpenSession())
                    {
                        session.Advanced.AllowNonAuthoritativeInformation = false;
                        Assert.Equal("Bread", session.Load<Item>(1).Name);
                    }
                }
            }
        }

        public class Item
        {
            public string Name;
        }
    }
}
