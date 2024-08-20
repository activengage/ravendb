using Raven35.Abstractions.Counters.Notifications;
using Raven35.Database.Server.Connections;

namespace Raven35.Database.Counters.Notifications
{
    public class NotificationPublisher
    {
        private readonly TransportState transportState;

        public NotificationPublisher(TransportState transportState)
        {
            this.transportState = transportState;
        }

        public void RaiseNotification(ChangeNotification notification)
        {
            transportState.Send(notification);
        }

        public void RaiseNotification(BulkOperationNotification change)
        {
            transportState.Send(change);
        }
    }
}
