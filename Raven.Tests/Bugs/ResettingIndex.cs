using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class ResettingIndex : RavenTest
    {
        [Fact]
        public void CanResetIndex()
        {
            using (var store = NewDocumentStore())
            {
                var ravenDocumentsByEntityName = new RavenDocumentsByEntityName();
                ravenDocumentsByEntityName.Execute(store);
                store.SystemDatabase.Indexes.ResetIndex(ravenDocumentsByEntityName.IndexName);
            }
        }
    }
}
