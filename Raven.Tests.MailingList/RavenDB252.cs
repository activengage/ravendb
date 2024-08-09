using Lucene.Net.Util;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;
using Constants = Raven35.Abstractions.Data.Constants;
using System.Linq;

namespace Raven35.Tests.MailingList
{
    public class RavenDB252 : RavenTest
    {
        [Fact]
        public void EntityNameIsNowCaseInsensitive()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.Put("a", null, new RavenJObject
                {
                    {"FirstName", "Oren"}
                }, new RavenJObject
                {
                    {Constants.RavenEntityName, "Users"}
                });

                store.DatabaseCommands.Put("b", null, new RavenJObject
                {
                    {"FirstName", "Ayende"}
                }, new RavenJObject
                {
                    {Constants.RavenEntityName, "users"}
                });

                using(var session = store.OpenSession())
                {
                    Assert.NotEmpty(session.Query<User>().Where(x=>x.FirstName == "Oren"));

                    Assert.NotEmpty(session.Query<User>().Where(x => x.FirstName == "Ayende"));
                }
            }
        }
        
        [Fact]
        public void EntityNameIsNowCaseInsensitive_Method()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.Put("a", null, new RavenJObject
                {
                    {"FirstName", "Oren"}
                }, new RavenJObject
                {
                    {Constants.RavenEntityName, "Users"}
                });

                store.DatabaseCommands.Put("b", null, new RavenJObject
                {
                    {"FirstName", "Ayende"}
                }, new RavenJObject
                {
                    {Constants.RavenEntityName, "users"}
                });

                WaitForIndexing(store);

                store.DatabaseCommands.PutIndex("UsersByName", new IndexDefinition
                {
                    Map = "docs.users.Select(x=>new {x.FirstName })"
                });

                WaitForIndexing(store);

                using (var session = store.OpenSession())
                {
                    Assert.NotEmpty(session.Query<User>("UsersByName").Customize(x=>x.WaitForNonStaleResults()).Where(x => x.FirstName == "Oren"));

                    Assert.NotEmpty(session.Query<User>("UsersByName").Where(x => x.FirstName == "Ayende"));
                }
            }
        }
    }
}
