using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Counters;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection.Async;
using Raven35.Client.Connection.Implementation;
using Raven35.Client.Counters.Operations;
using Raven35.Database.Counters;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Json.Linq;

namespace Raven35.Client.Counters
{
    public partial class CounterStore
    {
        public class CounterStoreAdvancedOperations
        {
            private readonly CounterStore parent;

            internal CounterStoreAdvancedOperations(CounterStore parent)
            {
                this.parent = parent;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public async Task<IReadOnlyList<CounterSummary>> GetCounters(int skip = 0, int take = 1024, CancellationToken token = default(CancellationToken))
            {
                return await parent.Admin.GetCountersByStorage(null,token,skip,take).ConfigureAwait(false);
            }

            public async Task<IReadOnlyList<CounterSummary>> GetCountersByPrefix(string groupName, string counterNamePrefix = null, int skip = 0, int take = 1024, CancellationToken token = default(CancellationToken))
            {
                if(string.IsNullOrWhiteSpace(groupName))
                    throw new ArgumentNullException(nameof(groupName));

                parent.AssertInitialized();
                await parent.ReplicationInformer.UpdateReplicationInformationIfNeededAsync().ConfigureAwait(false);

                var summaries = await parent.ReplicationInformer.ExecuteWithReplicationAsync(parent.Url, HttpMethods.Get, async (url, counterStoreName) =>
                {
                    var requestUriString = $"{url}/cs/{counterStoreName}/by-prefix?skip={skip}&take={take}&groupName={groupName}";
                    if (!string.IsNullOrWhiteSpace(counterNamePrefix))
                        requestUriString += $"&counterNamePrefix={counterNamePrefix}";

                    using (var request = parent.CreateHttpJsonRequest(requestUriString, HttpMethods.Get))
                    {
                        var response = await request.ReadResponseJsonAsync().WithCancellation(token).ConfigureAwait(false);
                        return response.ToObject<List<CounterSummary>>();
                    }
                }, token).ConfigureAwait(false);

                return summaries;
            }

            public CountersBatchOperation NewBatch(CountersBatchOptions options = null)
            {
                if (parent.Name == null)
                    throw new ArgumentException("Counter Storage isn't set!");

                parent.AssertInitialized();

                return new CountersBatchOperation(parent, parent.Name, options);
            }

            public async Task<List<CounterState>> GetCounterStatesSinceEtag(long etag, int skip = 0, int take = 1024, CancellationToken token = default(CancellationToken))
            {
                parent.AssertInitialized();
                await parent.ReplicationInformer.UpdateReplicationInformationIfNeededAsync().ConfigureAwait(false);

                var states = await parent.ReplicationInformer.ExecuteWithReplicationAsync(parent.Url, HttpMethods.Get,async (url, counterStoreName) =>
                {
                    var requestUriString = $"{url}/cs/{counterStoreName}/sinceEtag?etag={etag}&skip={skip}&take={take}";

                    using (var request = parent.CreateHttpJsonRequest(requestUriString, HttpMethods.Get))
                    {
                        var response = await request.ReadResponseJsonAsync().WithCancellation(token).ConfigureAwait(false);
                        return response.ToObject<List<CounterState>>();
                    }
                }, token).ConfigureAwait(false);
                
                return states;
            }

            
        }
    }
}
