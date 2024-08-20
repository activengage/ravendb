using Raven35.Abstractions.FileSystem;

namespace Raven35.Database.FileSystem.Notifications
{
    public interface INotificationPublisher
    {
        void Publish(FileSystemNotification change);
    }
}
