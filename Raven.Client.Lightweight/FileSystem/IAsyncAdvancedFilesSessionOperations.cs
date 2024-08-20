using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.FileSystem;
using Raven35.Abstractions.Util;

namespace Raven35.Client.FileSystem
{
    public interface IAsyncAdvancedFilesSessionOperations : IAdvancedFilesSessionOperations
    {
        /// <summary>
        ///     Stream the file headers
        ///     <para>Does NOT track the entities in the session, and will not includes changes there when SaveChangesasync() is called</para>
        /// </summary>
        /// <param name="fromEtag">ETag of a file from which stream should start</param>
        /// <param name="pageSize">Maximum number of file headers that will be retrieved</param>
        Task<IAsyncEnumerator<FileHeader>> StreamFileHeadersAsync(Etag fromEtag, int pageSize = int.MaxValue);

        /// <summary>
        ///     Stream the results on the query to the client
        ///     <para>Does NOT track the entities in the session, and will not includes changes there when SaveChangesasync() is called</para>
        /// </summary>
        /// <param name="query">Query to stream results for</param>
        Task<IAsyncEnumerator<FileHeader>> StreamQueryAsync(IAsyncFilesQuery<FileHeader> query);
    }
}
