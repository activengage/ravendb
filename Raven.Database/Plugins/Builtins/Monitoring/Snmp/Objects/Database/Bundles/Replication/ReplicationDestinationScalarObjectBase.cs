// -----------------------------------------------------------------------
//  <copyright file="ReplicationBundleScalarObjectBase.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;

using Lextm.SharpSnmpLib;

using Raven35.Bundles.Replication.Tasks;
using Raven35.Client.Connection;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.Plugins.Builtins.Monitoring.Snmp.Objects.Database.Bundles.Replication
{
    public abstract class ReplicationDestinationScalarObjectBase<TData> : DatabaseBundleScalarObjectBase<TData>
        where TData : ISnmpData
    {
        protected string DestinationUrl { get; private set; }

        protected ReplicationDestinationScalarObjectBase(string databaseName, DatabasesLandlord landlord, int databaseIndex, string destinationUrl, int destinationIndex, string dots)
            : base(databaseName, "Replication", landlord, databaseIndex, 1, string.Format("2.{0}.{1}", destinationIndex, dots))
        {
            DestinationUrl = destinationUrl;
        }

        protected override TData GetData(DocumentDatabase database)
        {
            var task = database.StartupTasks.OfType<ReplicationTask>().FirstOrDefault();
            if (task == null)
                return default(TData);

            var destinations = task.GetReplicationDestinations(destination => string.Equals(destination.Url.ForDatabase(destination.Database), DestinationUrl));
            if (destinations == null || destinations.Length == 0)
                return default(TData); ;

            return GetData(database, task, destinations[0]);
        }

        public abstract TData GetData(DocumentDatabase database, ReplicationTask task, ReplicationStrategy destination);
    }
}
