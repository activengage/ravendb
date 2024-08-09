using Raven35.Client.Document;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class LinusK : RavenTest
    {
        [Fact]
        public void RavenShouldNotModifyDatesValues()
        {
            using(GetNewServer())
            using(var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                store.DatabaseCommands.Put("test/1", null, RavenJObject.Parse(@"{ Timestamp: '\/Date(1340269200000+0200)\/'}"),
                                           new RavenJObject());

                var jsonDocument = store.DatabaseCommands.Get("test/1");
                Assert.Equal(@"/Date(1340269200000+0200)/", jsonDocument.DataAsJson.Value<string>("Timestamp"));
            }
        }
    }
}
