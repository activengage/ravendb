using System.IO;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.FileSystem.Bugs
{
    public class CaseSensitiveFileDeletion : RavenFilesTestWithLogs
    {
        [Fact]
        public void FilesWithUpperCaseNamesAreDeletedProperly()
        {
            var client = NewAsyncClient();
            var ms = new MemoryStream();
            client.UploadAsync("Abc.txt", ms).Wait();

            client.DeleteAsync("Abc.txt").Wait();

            var result = client.SearchOnDirectoryAsync("/").Result;

            Assert.Equal(0, result.FileCount);
        }
    }
}
