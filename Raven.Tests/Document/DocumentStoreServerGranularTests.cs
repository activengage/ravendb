using System.Net;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Document
{
    public class DocumentStoreServerGranularTests : NoDisposalNeeded
    {
        [Fact]
        public void Can_read_credentials_from_connection_string()
        {
            using (var documentStore = new DocumentStore { ConnectionStringName = "Secure" })
            {
                Assert.NotNull(documentStore.Credentials);
                var networkCredential = (NetworkCredential)documentStore.Credentials;
                Assert.Equal("beam", networkCredential.UserName);
                Assert.Equal("up", networkCredential.Password);
            }
        } 
    }
}
