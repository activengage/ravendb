using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.MailingList
{
    public class SpecialChars : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Query<User>()
                        .Where(x => x.LastName == "abc&edf")
                        .ToList();
                }
            }
        }

        [Fact]
        public void ShouldWork_Remote()
        {
            using(GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    session.Query<User>()
                        .Where(x => x.LastName == "abc&edf")
                        .ToList();
                }
            }
        }
    }
}
