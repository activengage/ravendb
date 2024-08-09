using System.Linq;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class IndexMetadata : RavenTest
    {
        public class Users_DeleteStatus : AbstractMultiMapIndexCreationTask
        {
            public Users_DeleteStatus()
            {
                AddMap<User>(users => from user in users
                                      select new
                                      {
                                          Deleted = MetadataFor(user)["Deleted"]
                                      });
            }
        }

        [Fact]
        public void WillGenerateProperIndex()
        {
            var usersDeleteStatus = new Users_DeleteStatus {Conventions = new DocumentConvention()};
            var indexDefinition = usersDeleteStatus.CreateIndexDefinition();
            Assert.Contains("Deleted = user[\"@metadata\"][\"Deleted\"]", indexDefinition.Map);
        }

        [Fact]
        public void CanCreateIndex()
        {
            using(var store = NewDocumentStore())
            {
                new Users_DeleteStatus().Execute(store);
            }
        }
    }
}
