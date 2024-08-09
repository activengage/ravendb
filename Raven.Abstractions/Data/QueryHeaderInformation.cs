using System;

namespace Raven35.Abstractions.Data
{
    public class QueryHeaderInformation
    {
        public string Index { get; set; }
        public bool IsStale { get; set; }
        public DateTime IndexTimestamp { get; set; }
        public int TotalResults { get; set; }
        public Etag ResultEtag { get; set; }
        public Etag IndexEtag { get; set; }
    }
}
