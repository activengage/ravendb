// -----------------------------------------------------------------------
//  <copyright file="ReplicationBundleEnabled.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;

using Lextm.SharpSnmpLib;

using Raven35.Bundles.Replication.Tasks;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.Plugins.Builtins.Monitoring.Snmp.Objects.Database.Bundles.Replication
{
    public class ReplicationBundleEnabled : DatabaseScalarObjectBase<OctetString>
    {
        public ReplicationBundleEnabled(string databaseName, DatabasesLandlord landlord, int index)
            : base(databaseName, landlord, "5.2.{0}.6.1.1", index)
        {
        }

        protected override OctetString GetData(DocumentDatabase database)
        {
            var task = database.StartupTasks.OfType<ReplicationTask>().FirstOrDefault();
            var enabled = task != null;
            return new OctetString(enabled.ToString());
        }
    }
}
