using System.Collections.Generic;
using System.Linq;

using Raven35.Abstractions.Replication;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Client.Shard;
using Raven35.Server;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class LazilyLoadWithTransformerWhileUsingSharding : RavenTest
    {
        private new readonly Dictionary<string, RavenDbServer> servers;
        private readonly ShardedDocumentStore store;

        public LazilyLoadWithTransformerWhileUsingSharding()
        {
            servers = new Dictionary<string, RavenDbServer>
            {
                {"shard", GetNewServer(8078)}
            };

            var documentStores = new Dictionary<string, IDocumentStore>
                                        {
                                            {"shard", new DocumentStore{Url = "http://localhost:8078"}}
                                        };

            foreach (var documentStore in documentStores)
            {
                documentStore.Value.Conventions.FailoverBehavior = FailoverBehavior.FailImmediately;
            }


            var shardStrategy = new ShardStrategy(documentStores);

            store = new ShardedDocumentStore(shardStrategy);
            store.Initialize();

            new TestTransformer().Execute(store);
        }

        [Fact]
        public void LazyLoadWithTransformerInShardedSetup()
        {
            using (var session = store.OpenSession())
            {
                var testDoc = new TestDocument();
                session.Store(testDoc);

                session.SaveChanges();

                var result = session.Advanced.Lazily.Load<TestTransformer, TestDto>(testDoc.Id);
                Assert.NotNull(result.Value);
            }
        }

        [Fact]
        public void LoadWithTransformerInShardedSetup()
        {
            using (var session = store.OpenSession())
            {
                var testDoc = new TestDocument();
                session.Store(testDoc);

                session.SaveChanges();

                var result = session.Load<TestTransformer, TestDto>(testDoc.Id);
                Assert.NotNull(result);
            }
        }

        public class TestDocument
        {
            public string Id { get; set; }
        }

        public class TestDto
        {
            public string Test { get; set; }
        }


        public class TestTransformer : AbstractTransformerCreationTask<TestDocument>
        {
            public TestTransformer()
            {
                TransformResults = contacts => from c in contacts
                                               select new
                                               {
                                                   Test = "test"
                                               };
            }
        }

        public override void Dispose()
        {
            foreach (var ravenDbServer in servers)
            {
                ravenDbServer.Value.Dispose();
            }
            store.Dispose();
            base.Dispose();
        }

    }
}
