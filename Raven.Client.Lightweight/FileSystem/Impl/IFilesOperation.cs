using Raven35.Abstractions.FileSystem;
using System.Threading.Tasks;

namespace Raven35.Client.FileSystem.Impl
{
    internal interface IFilesOperation 
    {
        string FileName { get; }

        Task<FileHeader> Execute(IAsyncFilesSession session);
    }
}
