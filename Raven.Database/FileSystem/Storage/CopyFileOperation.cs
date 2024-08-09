using Raven35.Json.Linq;
using System.Collections.Specialized;

namespace Raven35.Database.FileSystem.Storage
{
    public class CopyFileOperation
    {
        public string FileSystem { get; set; }
        public string SourceFilename { get; set; }

        public string TargetFilename { get; set; }

        public RavenJObject MetadataAfterOperation { get; set; }
    }
}
