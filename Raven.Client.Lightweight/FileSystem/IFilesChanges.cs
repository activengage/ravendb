using Raven35.Abstractions.FileSystem.Notifications;
using Raven35.Client.Changes;

namespace Raven35.Client.FileSystem
{
    public interface IFilesChanges : IConnectableChanges<IFilesChanges>
    {
        IObservableWithTask<ConfigurationChangeNotification> ForConfiguration();
        IObservableWithTask<ConflictNotification> ForConflicts();
        IObservableWithTask<FileChangeNotification> ForFolder(string folder);
        IObservableWithTask<SynchronizationUpdateNotification> ForSynchronization();
    }
}
