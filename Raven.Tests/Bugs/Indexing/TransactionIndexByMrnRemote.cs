using System.ComponentModel.Composition.Hosting;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class TransactionIndexByMrnRemote : RavenTest
    {
        [Fact]
        public void CanCreateIndex()
        {
            using(GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(typeof(Transaction_ByMrn))), store);
            }
        }
    }
}
