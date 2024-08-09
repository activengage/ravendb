using Raven35.Json.Linq;

namespace Raven35.Database.Impl
{
    public class CachedDocument
    {
        public int Size { get; set; }
        public RavenJObject Metadata { get; set; }
        public RavenJObject Document { get; set; }
    }
}
