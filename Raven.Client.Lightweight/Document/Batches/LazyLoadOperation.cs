using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Raven35.Abstractions.Data;
using Raven35.Client.Connection;
using Raven35.Client.Document.SessionOperations;
using Raven35.Client.Shard;
using Raven35.Json.Linq;

namespace Raven35.Client.Document.Batches
{
    public class LazyLoadOperation<T> : ILazyOperation
    {
        private readonly string key;
        private readonly LoadOperation loadOperation;
        private readonly Action<RavenJObject> handleInternalMetadata;

        public LazyLoadOperation(string key, LoadOperation loadOperation, Action<RavenJObject> handleInternalMetadata)
        {
            this.key = key;
            this.loadOperation = loadOperation;
            this.handleInternalMetadata = handleInternalMetadata;
        }

        public GetRequest CreateRequest()
        {
            const string path = "/docs";
            var query = "id=" + Uri.EscapeDataString(key);
            return new GetRequest
            {
                Url = path,
                Query = query
            };
        }

        public object Result { get; set; }

        public QueryResult QueryResult { get; set; }

        public bool RequiresRetry { get; set; }

        public void HandleResponses(GetResponse[] responses, ShardStrategy shardStrategy)
        {
            var response = responses.OrderBy(x => x.Status).First(); // this way, 200 response is higher than 404
            HandleResponse(response);
        }

        public void HandleResponse(GetResponse response)
        {
            if (response.ForceRetry)
            {
                Result = null;
                RequiresRetry = true;
                return;
            }

            if (response.Status == 404)
            {
                Result = null;
                RequiresRetry = false;
                return;
            }

            var headers = new NameValueCollection();
            foreach (var header in response.Headers)
            {
                headers[header.Key] = header.Value;
            }
            var jsonDocument = SerializationHelper.DeserializeJsonDocument(key, response.Result, headers, (HttpStatusCode)response.Status);
            HandleResponse(jsonDocument);
        }

        private void HandleResponse(JsonDocument jsonDocument)
        {
            RequiresRetry = loadOperation.SetResult(jsonDocument);
            if (RequiresRetry == false)
                Result = loadOperation.Complete<T>();
        }

        public IDisposable EnterContext()
        {
            return loadOperation.EnterLoadContext();
        }
        }
            }
