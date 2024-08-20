using System;

using Raven35.Abstractions.Util;
using Raven35.Database.Config;
using Raven35.Database.Server.Connections;

namespace Raven35.Database.Common
{
    public interface IResourceStore : IDisposable
    {
        string Name { get; }

        string ResourceName { get; }

        TransportState TransportState { get; }

        AtomicDictionary<object> ExtensionsState { get; }

        InMemoryRavenConfiguration Configuration { get; }
    }
}
