using System;
using Raven35.Abstractions.Data;
using Raven35.Client.Shard;

namespace Raven35.Client.Document.Batches
{
    public interface ILazyOperation
    {
        GetRequest CreateRequest();
        object Result { get; }
        QueryResult QueryResult { get; }
        bool RequiresRetry { get; }
        void HandleResponse(GetResponse response);
        void HandleResponses(GetResponse[] responses, ShardStrategy shardStrategy);

        IDisposable EnterContext();
    }
}
