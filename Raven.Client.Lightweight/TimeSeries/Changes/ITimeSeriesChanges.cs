using System;
using Raven35.Abstractions.TimeSeries.Notifications;
using Raven35.Client.Changes;

namespace Raven35.Client.TimeSeries.Changes
{
    public interface ITimeSeriesChanges : IConnectableChanges<ITimeSeriesChanges>
    {
        /// <summary>
        /// Subscribe to changes for specified type and key only.
        /// </summary>
        IObservableWithTask<TimeSeriesChangeNotification> ForKey(string type, string key);

        /// <summary>
        /// Subscribe to all bulk operation changes that belong to a operation with given Id.
        /// </summary>
        IObservableWithTask<BulkOperationNotification> ForBulkOperation(Guid? operationId = null);
    }
}
