using System;
using System.Collections.Generic;
using System.Linq;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Client.Shard;
using Raven35.Tests.Helpers;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3488 : RavenTestBase
    {
        public class Profile
        {
            public string Id { get; set; }
            
            public string Name { get; set; }

            public string Location { get; set; }
        }

        public class SomeShardingResolutionStrategy : DefaultShardResolutionStrategy
        {
            public SomeShardingResolutionStrategy(IEnumerable<string> shardIds, ShardStrategy shardStrategy)
                : base(shardIds, shardStrategy)
            {
            }

            public override IList<string> PotentialShardsFor(ShardRequestData requestData, List<string> potentialShardIds)
            {
                if (potentialShardIds == null)
                    return null;

                return new[] { potentialShardIds.Last() };
            }
        }

       public class ProfileIndex: AbstractIndexCreationTask
        {
            public override IndexDefinition CreateIndexDefinition()
            {
                return new IndexDefinition
                {
                    Map = @"from profile in docs select new { profile.Id, profile.Name, profile.Location };",                    
                };
            }
        }

        [Fact]
        public void ShouldWork()
        {
            var server1 = GetNewServer(8079);
            var server2 = GetNewServer(8078);
            var shards = new Dictionary<string, IDocumentStore>
            {
                {"Shard1", new DocumentStore{Url = server1.Configuration.ServerUrl}},
                {"Shard2", new DocumentStore{Url = server2.Configuration.ServerUrl}},
            };

            var shardStrategy = new ShardStrategy(shards);
            shardStrategy.ShardResolutionStrategy = new SomeShardingResolutionStrategy(shards.Keys, shardStrategy);
            shardStrategy.ShardingOn<Profile>(x => x.Location);

            using (var shardedDocumentStore = new ShardedDocumentStore(shardStrategy))
            {
                shardedDocumentStore.Initialize();
                new ProfileIndex().Execute(shardedDocumentStore);

                server1.Options.RequestManager.ResetNumberOfRequests();
                server2.Options.RequestManager.ResetNumberOfRequests();

                Assert.Equal(0, server1.Options.RequestManager.NumberOfRequests);
                Assert.Equal(0, server2.Options.RequestManager.NumberOfRequests);

                using (var session = shardedDocumentStore.OpenSession())
                {
                    var query = session
                        .Query<Profile>("ProfileIndex")
                        .Where(x => x.Location == "Shard1" || x.Location == "Shard2")
                        .ToList();

                    Assert.Equal(0, server1.Options.RequestManager.NumberOfRequests);
                    Assert.Equal(1, server2.Options.RequestManager.NumberOfRequests);
                }
            }
        }
    }
}
