using System.Threading.Tasks;
using Raven35.Client.Connection;

namespace Raven35.Client.FileSystem.Connection
{
    public interface IFilesReplicationInformer : IReplicationInformerBase<IAsyncFilesCommands>
    {
        /// <summary>
        /// Updates replication information if needed
        /// </summary>
        Task UpdateReplicationInformationIfNeeded(IAsyncFilesCommands commands);
    }
}
