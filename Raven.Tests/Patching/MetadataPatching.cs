
using Raven35.Abstractions.Data;
using Raven35.Database.Data;
using Raven35.Database.Json;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Patching
{
    public class MetadataPatching : RavenTest
    {
        [Fact]
        public void ChangeRavenEntityName()
        {
            using (var store = NewDocumentStore())
            {
                store.SystemDatabase.Documents.Put("foos/1", null, RavenJObject.Parse("{'Something':'something'}"),
                    RavenJObject.Parse("{'Raven-Entity-Name': 'Foos'}"), null);
                WaitForIndexing(store);
                var operation = store.DatabaseCommands.UpdateByIndex("Raven/DocumentsByEntityName",
                    new IndexQuery(), new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Modify,
                            Name = "@metadata",
                            Nested = new []
                            {
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Raven-Entity-Name",
                                    Value = new RavenJValue("Bars")
                                }
                            }
                        }
                            
                    }, null);

                operation.WaitForCompletion();

                var jsonDocument = store.SystemDatabase.Documents.Get("foos/1", null);
                Assert.Equal("Bars", jsonDocument.Metadata.Value<string>("Raven-Entity-Name"));
            }
        }
    }
}
