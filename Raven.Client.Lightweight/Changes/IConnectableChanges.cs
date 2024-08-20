using System;

namespace Raven35.Client.Changes
{
    public interface IConnectableChanges
    {
        bool Connected { get; }
        event EventHandler ConnectionStatusChanged;
        void WaitForAllPendingSubscriptions();
    }
}
