using System;
using Raven35.Client.Connection;

namespace Raven35.Client.FileSystem.Connection
{
    public interface IAsyncFilesCommandsImpl : IAsyncFilesCommands
    {   
        string ServerUrl { get; }

        HttpJsonRequestFactory RequestFactory { get; }

        IFilesReplicationInformer ReplicationInformer { get; }
    }
}
