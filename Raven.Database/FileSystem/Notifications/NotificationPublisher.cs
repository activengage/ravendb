using Raven35.Abstractions.FileSystem;
using Raven35.Database.Server.Connections;

namespace Raven35.Database.FileSystem.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly TransportState transportState;

        public NotificationPublisher(TransportState transportState)
        {
            this.transportState = transportState;
        }

        public void Publish(FileSystemNotification change)
        {
            transportState.Send(change);
        }
    }
}
