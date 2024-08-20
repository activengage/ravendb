using Raven35.Abstractions.Data;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_187 : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.Put("users/1", null, new RavenJObject(), new RavenJObject
                {
                    {Constants.RavenDeleteMarker, "true"}
                });

                using (var s = store.OpenSession())
                {
                    s.Advanced.UseOptimisticConcurrency = true;
                    s.Store(new User());
                    s.SaveChanges();
                }
            }
        }
    }
}
