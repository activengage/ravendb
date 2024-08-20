using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Linq;
using Raven35.Tests.Bugs;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.MultiGet
{
    public class Bugs : RavenTest
    {
        [Fact]
        public void CanUseStats()
        {
            using (GetNewServer())
            using (var docStore = new DocumentStore { Url = "http://localhost:8079" }.Initialize())
            {
                using (var session = docStore.OpenSession())
                {
                    session.Store(new User { Name = "Ayende" });
                    session.Store(new User { Name = "Oren" });
                    session.SaveChanges();
                }


                using (var session = docStore.OpenSession())
                {
                    RavenQueryStatistics stats;
                    session.Query<User>()
                        .Customize(x=>x.WaitForNonStaleResults())
                        .Statistics(out stats)
                        .Lazily();

                    session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

                    Assert.Equal(2, stats.TotalResults);
                }
            }
        }
    }
}
