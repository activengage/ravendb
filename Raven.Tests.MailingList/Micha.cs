using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Indexes;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Micha : RavenTest
    {
        public class Entity
        {
            public string Label { get; set; }	
        }

        public class EntityEntityIdPatch : AbstractIndexCreationTask<Entity>
        {
            public EntityEntityIdPatch()
            {
                Map = docs => from doc in docs
                              select new { doc.Label };
            }
        }

        [Fact]
        public void CanDeleteIndex()
        {
            using(var store = NewDocumentStore())
            {
                new EntityEntityIdPatch().Execute(store);

                WaitForIndexing(store);

                store.DatabaseCommands.UpdateByIndex("EntityEntityIdPatch",
                                                     new IndexQuery(),
                                                     new[]
                                                     {
                                                        new PatchRequest()
                                                        {
                                                            Type = PatchCommandType.Rename,
                                                            Name = "EntityType",
                                                            Value = new RavenJValue("EntityTypeId")
                                                        }
                                                     }, null);
                var id = store.SystemDatabase.IndexDefinitionStorage.GetIndexDefinition("EntityEntityIdPatch").IndexId;
                store.DatabaseCommands.DeleteIndex("EntityEntityIdPatch");

                Assert.False(store.SystemDatabase.Statistics.Indexes.Any(x => x.Id == id));
            }
        }
    }
}
