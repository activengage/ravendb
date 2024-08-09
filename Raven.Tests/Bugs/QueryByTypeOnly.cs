using System.Linq;

using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class QueryByTypeOnly : RavenTest
    {
        [Fact]
        public void QueryOnlyByType()
        {
            using(var store = NewDocumentStore())
            {
                using(var session = store.OpenSession())
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
