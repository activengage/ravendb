using Raven35.Abstractions.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_505 : RavenTest
    {
        [Fact]
        public void CreateDeleteCreateIndex()
        {
            using (var store = NewDocumentStore(requestedStorage:"esent"))
            {
                var indexDefinition = new IndexDefinition
                {
                    Map = "from d in docs select new {}"
                };
                for (int i = 0; i < 10; i++)
                {
                    store.DatabaseCommands.PutIndex("test", indexDefinition);
                    store.DatabaseCommands.DeleteIndex("test");
                }
            }
        }


    }
}
