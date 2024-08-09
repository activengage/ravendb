// -----------------------------------------------------------------------
//  <copyright file="ReplicationDestinationEnabled.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Lextm.SharpSnmpLib;

using Raven35.Bundles.Replication.Tasks;
using Raven35.Client.Connection;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.Plugins.Builtins.Monitoring.Snmp.Objects.Database.Bundles.Replication
{
    public class ReplicationDestinationUrl : ReplicationDestinationScalarObjectBase<OctetString>
    {
        public ReplicationDestinationUrl(string databaseName, DatabasesLandlord landlord, int databaseIndex, string destinationUrl, int destinationIndex)
            : base(databaseName, landlord, databaseIndex, destinationUrl, destinationIndex, "2")
        {
        }

        public override OctetString GetData(DocumentDatabase database, ReplicationTask task, ReplicationStrategy destination)
        {
            return new OctetString(destination.ConnectionStringOptions.Url.ForDatabase(destination.ConnectionStringOptions.DefaultDatabase));
        }
    }
}
