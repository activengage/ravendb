using System.IO;
using System.Threading.Tasks;

namespace Raven35.Database.FileSystem.Synchronization.Rdc
{
    public interface IPartialDataAccess
    {
        Task CopyToAsync(Stream target, long from, long length);
    }
}
