using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.TimeSeries;
using Raven35.Abstractions.Util;
using Raven35.Client;
using Raven35.Client.TimeSeries;
using Raven35.Database.Extensions;
using Raven35.Server;
using Raven35.Tests.Helpers;

namespace Raven35.Tests.TimeSeries
{
    public class RavenBaseTimeSeriesTest : RavenTestBase
    {
        protected readonly List<TimeSeriesStore> timeSeriesStores = new List<TimeSeriesStore>();

        protected readonly string DefaultTimeSeriesName = "SeriesName-";

        protected RavenBaseTimeSeriesTest()
        {
            foreach (var folder in Directory.EnumerateDirectories(Directory.GetCurrentDirectory(), "ThisIsRelativelyUniqueTimeSeriesName*"))
                IOExtensions.DeleteDirectory(folder);

            // DefaultTimeSeriesName += Guid.NewGuid();
        }

        protected ITimeSeriesStore NewRemoteTimeSeriesStore(int port = 8079, RavenDbServer ravenDbServer = null, bool createDefaultTimeSeries = true, OperationCredentials credentials = null)
        {
            ravenDbServer = GetNewServer(requestedStorage: "voron", databaseName: DefaultTimeSeriesName + "Database", port: port);

            var timeSeriesStore = new TimeSeriesStore
            {
                Url = GetServerUrl(true, ravenDbServer.SystemDatabase.ServerUrl),
                Credentials = credentials ?? new OperationCredentials(null,CredentialCache.DefaultNetworkCredentials),
                Name = DefaultTimeSeriesName + (timeSeriesStores.Count + 1)
            };

            timeSeriesStore.Initialize(createDefaultTimeSeries);
            timeSeriesStores.Add(timeSeriesStore);
            return timeSeriesStore;
        }

        public override void Dispose()
        {
            var errors = new List<Exception>();

            foreach (var store in timeSeriesStores)
            {
                try
                {
                    store.Dispose();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            stores.Clear();

            if (errors.Count > 0)
                throw new AggregateException(errors);

            base.Dispose();
        }

        protected static async Task SetupReplicationAsync(ITimeSeriesStore source, params ITimeSeriesStore[] destinations)
        {
            var replicationDocument = new TimeSeriesReplicationDocument();
            foreach (var destStore in destinations)
            {
                replicationDocument.Destinations.Add(new TimeSeriesReplicationDestination
                {
                    TimeSeriesName = destStore.Name,
                    ServerUrl = GetServerUrl(true, destStore.Url),
                });
            }

            await source.SaveReplicationsAsync(replicationDocument);
        }
    }
}
