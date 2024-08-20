// -----------------------------------------------------------------------
//  <copyright file="ServerName.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Lextm.SharpSnmpLib;

using Raven35.Database.Config;

namespace Raven35.Database.Plugins.Builtins.Monitoring.Snmp.Objects.Server
{
    public class ServerName : ScalarObjectBase<OctetString>
    {
        private readonly OctetString name;

        public ServerName(InMemoryRavenConfiguration configuration)
            : base("1.1")
        {
            name = new OctetString(configuration.ServerName ?? string.Empty);
        }

        protected override OctetString GetData()
        {
            return name;
        }
    }
}
