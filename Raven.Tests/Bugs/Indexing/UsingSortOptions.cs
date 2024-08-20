using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class UsingSortOptions : RavenTest
    {
        [Fact]
        public void CanCreateIndexWithSortOptionsOnStringVal()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = "from user in docs.Users select new { user.Name }",
                    SortOptions = {{"Name", SortOptions.StringVal}}
                });
                var indexDefinition = store.DatabaseCommands.GetIndex("test");

                Assert.Equal(SortOptions.StringVal, indexDefinition.SortOptions["Name"]);
            }
        }
    }
}
