using Raven35.Abstractions.FileSystem;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Json.Linq;
using Xunit;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.FileSystem.Notifications;
using Raven35.Abstractions.Data;

namespace Raven35.Tests.FileSystem.ClientApi
{
    public class SerializationTests
    {

        [Fact]
        public void FileHeaderSerialization()
        {
            var metadata = new RavenJObject { { Constants.LastModified, "2014-07-07T12:00:00.0000000" }, { Constants.FileSystem.RavenFsSize, "128" } };
            var fileHeader = new FileHeader("test1.file", metadata);

            var serializedValue = JsonExtensions.ToJObject(fileHeader);

            var jr = new RavenJTokenReader(serializedValue);
            var deserializedValue = JsonExtensions.CreateDefaultJsonSerializer().Deserialize<FileHeader>(jr);

            Assert.NotNull(deserializedValue);
            Assert.Equal(fileHeader.Name, deserializedValue.Name);
            Assert.Equal(fileHeader.LastModified, deserializedValue.LastModified);
        }

        [Fact]
        public void ConflictNotificationSerialization()
        {
            var metadata = new RavenJObject { { Constants.LastModified, "2014-07-07T12:00:00.0000000" }, { Constants.FileSystem.RavenFsSize, "128" } };
            var fileHeader = new FileHeader("test1.file", metadata);
            var notification = new ConflictNotification() { FileName = "test1.file", SourceServerUrl = "http://destination", RemoteFileHeader = fileHeader, Status = ConflictStatus.Detected};

            var serializedValue = JsonExtensions.ToJObject(notification);

            var jr = new RavenJTokenReader(serializedValue);
            var deserializedValue = JsonExtensions.CreateDefaultJsonSerializer().Deserialize<ConflictNotification>(jr);

            Assert.NotNull(deserializedValue);
        }
    }
}
