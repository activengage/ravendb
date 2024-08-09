using Raven35.Abstractions.Indexing;

namespace Raven35.Abstractions.Replication
{
    public class SideBySideReplicationInfo
    {
        public IndexDefinition Index { get; set; }

        public IndexDefinition SideBySideIndex { get; set; }

        public IndexReplaceDocument IndexReplaceDocument { get; set; }

        public string OriginDatabaseId { get; set; }
    }
}
