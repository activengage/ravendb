using System.Threading.Tasks;
using Raven35.Json.Linq;
using System.Collections.Specialized;
using System.IO;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.FileSystem
{
    public class CompleteUsage : RavenFilesTestWithLogs
    {
        [Fact]
        public async Task HowToUseTheClient()
        {
            var client = NewAsyncClient();
            var uploadTask = client.UploadAsync("dragon.design", new MemoryStream(new byte[] { 1, 2, 3 }), new RavenJObject
            {
                {"Customer", "Northwind"},
                {"Preferred", "True"}
            });

            await uploadTask; // or we can just let it run

            var search = await client.SearchAsync("Customer:Northwind AND Preferred:True");

            Assert.Equal("dragon.design", search.Files[0].Name);
        }
    }
}
