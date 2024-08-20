using System;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Counters;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.Util;
using Raven35.Json.Linq;

namespace Raven35.Client.Counters
{
    public partial class CounterStore
    {
        public async Task<CountersReplicationDocument> GetReplicationsAsync(CancellationToken token = default (CancellationToken))
        {
            AssertInitialized();

            var requestUriString = $"{Url}/cs/{Name}/replication/config";

            using (var request = CreateHttpJsonRequest(requestUriString, HttpMethods.Get))
            {
                var response = await request.ReadResponseJsonAsync().WithCancellation(token).ConfigureAwait(false);
                return response.ToObject<CountersReplicationDocument>(JsonSerializer);
            }
        }

        public async Task SaveReplicationsAsync(CountersReplicationDocument newReplicationDocument, CancellationToken token = default(CancellationToken))
        {
            AssertInitialized();
            var requestUriString = $"{Url}/cs/{Name}/replication/config";

            using (var request = CreateHttpJsonRequest(requestUriString, HttpMethods.Post))
            {
                await request.WriteAsync(RavenJObject.FromObject(newReplicationDocument)).WithCancellation(token).ConfigureAwait(false);
                await request.ReadResponseJsonAsync().WithCancellation(token).ConfigureAwait(false);
            }
        }

        public async Task<long> GetLastEtag(string serverId, CancellationToken token = default(CancellationToken))
        {
            AssertInitialized();
            var requestUriString = $"{Url}/cs/{Name}/lastEtag?serverId={serverId}";

            using (var request = CreateHttpJsonRequest(requestUriString, HttpMethods.Get))
            {
                var response = await request.ReadResponseJsonAsync().WithCancellation(token).ConfigureAwait(false);
                return response.Value<long>();
            }
        }
    }
}
