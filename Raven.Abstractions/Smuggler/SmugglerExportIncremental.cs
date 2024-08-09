using System;
using System.Collections.Generic;
using Raven35.Abstractions.Data;

namespace Raven35.Smuggler
{
    public class SmugglerExportIncremental
    {
        public const string RavenDocumentKey = "Raven35.Smuggler/Export/Incremental";

        public Dictionary<string, ExportIncremental> ExportIncremental { get; set; }

        public SmugglerExportIncremental()
        {
            ExportIncremental = new Dictionary<string, ExportIncremental>();
        }
    }

    public class ExportIncremental
    {
        public ExportIncremental()
        {
            LastDocsEtag = Etag.Empty;
            LastAttachmentsEtag = Etag.Empty;
        }

        public Etag LastDocsEtag { get; set; }

        [Obsolete("Use RavenFS instead.")]
        public Etag LastAttachmentsEtag { get; set; }

        public Etag LastFilesEtag { get; set; }
    }
}
