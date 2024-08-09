using System;

using Raven35.Abstractions.Logging;

namespace Raven35.Database.Server.Connections
{
    public interface ILogsTransport : IDisposable
    {
        string Id { get; }
        bool Connected { get; set; }

        event Action Disconnected;
        void SendAsync(LogEventInfo msg);
    }
}
