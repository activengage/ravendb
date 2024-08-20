// -----------------------------------------------------------------------
//  <copyright file="ServerName.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Lextm.SharpSnmpLib;

using Raven35.Database.Config;

namespace Raven35.Database.Plugins.Builtins.Monitoring.Snmp.Objects.Server
{
    public class ServerUrl : ScalarObjectBase<OctetString>
    {
        private readonly OctetString url;

        public ServerUrl(InMemoryRavenConfiguration configuration)
            : base("1.9")
        {
            url = new OctetString(configuration.ServerUrl);
        }

        protected override OctetString GetData()
        {
            return url;
        }
    }
}
