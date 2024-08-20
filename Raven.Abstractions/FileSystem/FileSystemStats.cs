using System;
using System.Collections.Generic;
using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.FileSystem
{
    public class FileSystemStats
    {
        public string Name { get; set; }

        public Guid FileSystemId { get; set; }

        public string ServerUrl { get; set; }

        public long FileCount { get; set; }

        public FileSystemMetrics Metrics { get; set; }

        public IList<SynchronizationDetails> ActiveSyncs { get; set; }

        public IList<SynchronizationDetails> PendingSyncs { get; set; }

        public Etag LastFileEtag { get; set; }
    }
}
