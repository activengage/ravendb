using System.Collections.Generic;
using Raven35.Client.Embedded;
using Raven35.Database.Config;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_10888
    {
        [Fact]
        public void WillWaitForIndexes()
        {
            using (var store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            })
            {
                store.Configuration.DefaultStorageTypeName = InMemoryRavenConfiguration.VoronTypeName;
                store.Configuration.Storage.Voron.AllowOn32Bits = true;
                store.Initialize();

                {
                    const string docId = "Entity/1";
                    using (var session = store.OpenSession())
                    {
                        var entity = new Entity();
                        session.Store(entity, docId);
                        session.SaveChanges();
                    }

                    using (var session = store.OpenSession())
                    {
                        var entity = session.Load<Entity>(docId);
                        session.Delete(entity);
                        session.Advanced.WaitForIndexesAfterSaveChanges();
                        session.SaveChanges();
                    }
                }
            }
        }

        internal class Entity
        {
            public string Id { get; set; }
            public List<string> Tokens { get; set; }
        }
    }
}
