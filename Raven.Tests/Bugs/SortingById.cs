using System.Linq;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class SortingById : RavenTest
    {
        [Fact]
        public void ShouldBePossible()
        {
            using (GetNewServer())
            using (IDocumentStore store = new DocumentStore {Url = "http://localhost:8079"}.Initialize())
            {
                using (IDocumentSession session = store.OpenSession())
                {
                    session.Query<SerializingEntities.Product>().OrderBy(w => w.Id).Skip(5).Take(5).ToArray();
                }
            }
        }
    }
}
