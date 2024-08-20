using System.ComponentModel.Composition.Hosting;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class TransactionIndexByMrn : RavenTest
    {
        [Fact]
        public void CanCreateIndex()
        {
            using (var store = NewDocumentStore())
            {
                IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(typeof(Transaction_ByMrn))), store);
            }
        }
    }
}
