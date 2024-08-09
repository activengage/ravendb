using Raven35.Json.Linq;

namespace Raven35.Database.Bundles.SqlReplication
{
    public class ItemToReplicate
    {
        public string DocumentId { get; set; }
        public RavenJObject Columns { get; set; }
    }
}
