using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Support;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Client.Shard;
using Raven35.Tests.Helpers;
using Raven35.Client.Linq;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class ShardingWithAsyncTransformer : RavenTestBase
    {
        public class Profile
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Location { get; set; }
        }

        public class Transformer : AbstractTransformerCreationTask<Profile>
        {
            public Transformer()
            {
                TransformResults = profiles =>
                    from profile in profiles
                    select new { profile.Name };
            }
        }

        public class Result
        {
            public string Name;
        }
        [Fact]
        public async Task CanUseAsyncTransformer()
        {
            var server1 = GetNewServer(8079);
            var server2 = GetNewServer(8078);
            var shards = new Dictionary<string, IDocumentStore>
            {
                {"Shard1", new DocumentStore{Url = server1.Configuration.ServerUrl}},
                {"Shard2", new DocumentStore{Url = server2.Configuration.ServerUrl}},
            };



            var shardStrategy = new ShardStrategy(shards);
            shardStrategy.ShardingOn<Profile>(x => x.Location);

            using (var shardedDocumentStore = new ShardedDocumentStore(shardStrategy))
            {
                shardedDocumentStore.Initialize();
                new Transformer().Execute(shardedDocumentStore);


                using (var session = shardedDocumentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(new Profile
                    {
                        Name = "Oren",
                        Location = "Shard1"
                    });
                    await session.SaveChangesAsync();
                }

                using (var session = shardedDocumentStore.OpenAsyncSession())
                {
                    var results = await session.Query<Profile>()
                        .Customize(x=>x.WaitForNonStaleResults())
                        .Where(x => x.Name == "Oren")
                        .TransformWith<Transformer, Result>()
                        .ToListAsync();

                    Assert.Equal("Oren", results[0].Name);
                }

            }
        }
    }
}
