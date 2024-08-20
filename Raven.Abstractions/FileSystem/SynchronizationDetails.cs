using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.FileSystem
{
    public class SynchronizationDetails
    {
        public string FileName { get; set; }
        public Etag FileETag { get; set; }
        public string DestinationUrl { get; set; }
        public SynchronizationType Type { get; set; }
    }
}
