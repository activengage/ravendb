using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.FileSystem;
using Raven35.Abstractions.Smuggler.Data;
using Raven35.Abstractions.Util;
using Raven35.Json.Linq;

namespace Raven35.Abstractions.Smuggler
{
    public interface ISmugglerFilesOperations
    {
        SmugglerFilesOptions Options { get; }


        Task<FileSystemStats[]> GetStats();
        Task<BuildNumber> GetVersion(FilesConnectionStringOptions server);        

        LastFilesEtagsInfo FetchCurrentMaxEtags();


        Task<IAsyncEnumerator<FileHeader>> GetFiles(Etag lastEtag, int take);
        Task<Stream> DownloadFile(FileHeader file);
        Task PutFile(FileHeader file, Stream data, long dataSize);


        Task<IEnumerable<KeyValuePair<string, RavenJObject>>> GetConfigurations(int start, int take);
        Task PutConfig(string name, RavenJObject value);
        
        void Initialize(SmugglerFilesOptions options);

        void Configure(SmugglerFilesOptions options);

        void ShowProgress(string format, params object[] args);

        string CreateIncrementalKey();
        Task<ExportFilesDestinations> GetIncrementalExportKey();
        Task PutIncrementalExportKey(ExportFilesDestinations destinations);

        RavenJObject StripReplicationInformationFromMetadata(RavenJObject metadata);

        RavenJObject DisableVersioning(RavenJObject metadata);
        Task<Stream> ReceiveFilesInStream(List<string> filePaths);

        bool IsEmbedded { get; }
        Task UploadFilesInStream(FileUploadUnitOfWork[] files);
    }
}
