using Raven35.Abstractions.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven35.Abstractions.Smuggler.Data
{
    public class ExportFilesDestinations
    {
        public const string RavenDocumentKey = "Raven35.Smuggler/Export/Files/Incremental";

        public Dictionary<string, ExportFilesDestinationKey> Destinations { get; set; }

        public ExportFilesDestinations()
        {
            Destinations = new Dictionary<string, ExportFilesDestinationKey>();
        }
    }

    public class ExportFilesDestinationKey
    {
        public Etag LastEtag { get; set; }

        public Etag LastDeletedEtag { get; set; }
    }
}
