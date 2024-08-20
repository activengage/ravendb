using System.Threading.Tasks;

namespace Raven35.Client.Changes
{
    public interface IConnectableChanges<T> : IConnectableChanges where T : IConnectableChanges
    {
        Task<T> Task { get; }
    }
}
