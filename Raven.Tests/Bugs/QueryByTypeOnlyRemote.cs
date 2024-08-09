using System.Linq;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class QueryByTypeOnlyRemote : RavenTest
    {
        [Fact]
        public void QueryOnlyByType()
        {
            using (GetNewServer())
            using (var store = new DocumentStore{Url = "http://localhost:8079"}.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    session.Query<SerializingEntities.Product>()
                        .Skip(5)
                        .Take(5)
                        .ToList();
                }
            }
        }
    }
}
