// -----------------------------------------------------------------------
//  <copyright file="DatabaseOpenedCount.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Lextm.SharpSnmpLib;

using Raven35.Abstractions.Data;
using Raven35.Database.Server.Tenancy;

namespace Raven35.Database.Plugins.Builtins.Monitoring.Snmp.Objects.Database.Statistics
{
    public class DatabaseActiveBundles : DatabaseScalarObjectBase<OctetString>
    {
        public DatabaseActiveBundles(string databaseName, DatabasesLandlord landlord, int index)
            : base(databaseName, landlord, "5.2.{0}.1.12", index)
        {
        }

        protected override OctetString GetData(DocumentDatabase database)
        {
            return new OctetString(database.Configuration.Settings[Constants.ActiveBundles] ?? string.Empty);
        }
    }
}
