using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Database.Config.Retriever
{
    public class ConfigurationDocument<TClass>
    {
        public bool LocalExists { get; set; }

        public bool GlobalExists { get; set; }

        public TClass MergedDocument { get; set; }

        public TClass GlobalDocument { get; set; }

        public Etag Etag { get; set; }

        public RavenJObject Metadata { get; set; }
    }
}
