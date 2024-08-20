using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.Smuggler
{
    public class SmugglerFilesOptions : SmugglerOptions<FilesConnectionStringOptions>
    {
        public SmugglerFilesOptions()
        {
            StartFilesEtag = Etag.Empty;
            StartFilesDeletionEtag = Etag.Empty;
        }

        public Etag StartFilesEtag { get; set; }
        public Etag StartFilesDeletionEtag { get; set; }

        public bool StripReplicationInformation { get; set; }

        public bool ShouldDisableVersioningBundle { get; set; }

        /// <summary>
        /// When set ovverides the default document name.
        /// </summary>
        public string NoneDefaultFileName { get; set; }
    }
}
