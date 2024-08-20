using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class WhenDoingSimpleLoad : RavenTest
    {
        protected override void CreateDefaultIndexes(Client.IDocumentStore documentStore)
        {
        }

        [Fact]
        public void WillMakeJustOneRequest()
        {
            using (var server = GetNewServer(databaseName: Constants.SystemDatabase))
            using (var documentStore = new DocumentStore
            {
                Url = "http://localhost:8079",
                Conventions =
                {
                    FailoverBehavior = FailoverBehavior.FailImmediately,
                }
            }.Initialize())
            {
                using (var session = documentStore.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    Assert.Null(user);
                }

                Assert.Equal(1, server.Server.NumberOfRequests);
            }
        }
    }
}
