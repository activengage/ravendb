using System;

using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.Indexing
{
    public class IndexReplaceDocument
    {
        public string IndexToReplace { get; set; }

        public Etag MinimumEtagBeforeReplace { get; set; }

        public DateTime? ReplaceTimeUtc { get; set; }
    }
}
