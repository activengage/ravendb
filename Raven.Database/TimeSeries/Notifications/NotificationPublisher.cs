using Raven35.Abstractions.TimeSeries.Notifications;
using Raven35.Database.Server.Connections;

namespace Raven35.Database.TimeSeries.Notifications
{
    public class NotificationPublisher
    {
        private readonly TransportState transportState;

        public NotificationPublisher(TransportState transportState)
        {
            this.transportState = transportState;
        }

        public void RaiseNotification(TimeSeriesChangeNotification notification)
        {
            transportState.Send(notification);
        }

        public void RaiseNotification(BulkOperationNotification change)
        {
            transportState.Send(change);
        }
    }
}