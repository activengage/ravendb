using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Stats : RavenTest
    {
        [Fact]
        public void Embedded()
        {
            using(var store = NewDocumentStore())
            {
                Assert.NotNull(store.DatabaseCommands.GetStatistics());
            }
        }

        [Fact]
        public void Server()
        {
            using(GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                Assert.NotNull(store.DatabaseCommands.GetStatistics());
            }
        }
    }
}
