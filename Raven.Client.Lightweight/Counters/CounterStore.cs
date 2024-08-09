using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Counters;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection;
using Raven35.Client.Connection.Implementation;
using Raven35.Client.Counters.Changes;
using Raven35.Client.Counters.Replication;
using Raven35.Client.Extensions;
using Raven35.Client.Util;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Json.Linq;

namespace Raven35.Client.Counters
{
    /// <summary>
    /// Implements client-side counters functionality
    /// </summary>
    public partial class CounterStore : ICounterStore
    {
        private readonly AtomicDictionary<ICountersChanges> counterStorageChanges = new AtomicDictionary<ICountersChanges>(StringComparer.OrdinalIgnoreCase);
        private CounterReplicationInformer replicationInformer;
        private bool isInitialized;
        public NameValueCollection OperationsHeaders { get; set; } = new NameValueCollection();

        public CounterStore()
        {
            JsonSerializer = JsonExtensions.CreateDefaultJsonSerializer();
            JsonRequestFactory = new HttpJsonRequestFactory(Constants.NumberOfCachedRequests);
            CountersConvention = new CountersConvention();
            Credentials = new OperationCredentials(null, CredentialCache.DefaultNetworkCredentials);
            Advanced = new CounterStoreAdvancedOperations(this);
            Admin = new CounterStoreAdminOperations(this);
            Stream = new CounterStreams(this);
            batch = new Lazy<BatchOperationsStore>(() => new BatchOperationsStore(this));
            isInitialized = false;
        }

        public void Initialize(bool ensureDefaultCounterExists = false)
        {
            if(isInitialized)
                throw new InvalidOperationException($"CounterStore already initialized. (name = {Name})");

            isInitialized = true;
            SecurityExtensions.InitializeSecurity(CountersConvention, JsonRequestFactory, Url);

            if (ensureDefaultCounterExists)
            {
                if (string.IsNullOrWhiteSpace(Name))
                    throw new InvalidOperationException("Name is null or empty and ensureDefaultCounterExists = true --> cannot create default counter storage with empty name");
                            
                if (!AsyncHelpers.RunSync(() => Admin.CounterStorageExists())) 
                {
                    //this statement will essentially overwrite the counter storage, therefore it should not be called if the storage is already there
                    Admin.CreateCounterStorageAsync(new CounterStorageDocument
                    {
                        Id = Constants.Counter.Prefix + Name,
                        Settings = new Dictionary<string, string>
                        {
                            {"Raven/Counters/DataDir", @"~\Counters\" + Name}
                        },
                    }, Name).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }			

            replicationInformer = new CounterReplicationInformer(JsonRequestFactory, this, CountersConvention); // make sure it is initialized
        }

        public ICountersChanges Changes(string counterStorage = null)
        {
            AssertInitialized();

            if (string.IsNullOrWhiteSpace(counterStorage))
                counterStorage = Name;

            return counterStorageChanges.GetOrAdd(counterStorage, CreateCounterStorageChanges);
        }		

        private ICountersChanges CreateCounterStorageChanges(string counterStorage)
        {
            if (string.IsNullOrEmpty(Url))
                throw new InvalidOperationException("Changes API requires usage of server/client");
            
            AssertInitialized();

            var tenantUrl = $"{Url}/cs/{counterStorage}";

            using (NoSynchronizationContext.Scope())
            {
                var client = new CountersChangesClient(tenantUrl,
                    Credentials.ApiKey,
                    Credentials.Credentials,
                    JsonRequestFactory,
                    CountersConvention,
                    () => counterStorageChanges.Remove(counterStorage));

                return client;
            }
        }

        public event EventHandler AfterDispose;

        public bool WasDisposed { get; private set; }

        internal void AssertInitialized()
        {
            if (!isInitialized)
                throw new InvalidOperationException("You cannot access the counters commands before initializing the counter store. " +
                                                    $"Did you forget calling Initialize()? (Counter store name = {Name})");
        }

        private readonly Lazy<BatchOperationsStore> batch;

        public BatchOperationsStore Batch => batch.Value;

        public OperationCredentials Credentials { get; set; }

        public HttpJsonRequestFactory JsonRequestFactory { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public CountersConvention CountersConvention { get; set; }

        internal JsonSerializer JsonSerializer { get; set; }

        public CounterStoreAdvancedOperations Advanced { get; private set; }

        public CounterStoreAdminOperations Admin { get; private set; }

        public CounterStreams Stream { get; private set; }

        private HttpJsonRequest CreateHttpJsonRequest(string requestUriString, HttpMethod httpMethod, bool disableRequestCompression = false, bool disableAuthentication = false)
        {
            return JsonRequestFactory.CreateHttpJsonRequest(new CreateHttpJsonRequestParams(null, 
                requestUriString, 
                httpMethod, 
                Credentials, 
                CountersConvention)
            {
                DisableRequestCompression = disableRequestCompression,
                DisableAuthentication = disableAuthentication
            });
        }

        public CounterReplicationInformer ReplicationInformer => 
            replicationInformer ?? (replicationInformer = new CounterReplicationInformer(JsonRequestFactory, this, CountersConvention));

        public void Dispose()
        {
            if(batch.IsValueCreated)
                batch.Value.Dispose();
        }
    }
}
