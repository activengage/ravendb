// -----------------------------------------------------------------------
//  <copyright file="ReplicationTopologyController.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Raven35.Database.Bundles.Replication.Impl;
using Raven35.Database.Server.Controllers;
using Raven35.Database.Server.WebApi.Attributes;
using Raven35.Json.Linq;

namespace Raven35.Database.Bundles.Replication.Controllers
{
    public class ReplicationTopologyController : BaseDatabaseApiController
    {
        [HttpPost]
        [RavenRoute("admin/replication/topology/discover")]
        [RavenRoute("databases/{databaseName}/admin/replication/topology/discover")]
        public async Task<HttpResponseMessage> ReplicationTopologyDiscover()
        {
            var ttlAsString = GetQueryStringValue("ttl");

            int ttl;
            RavenJArray from;

            if (string.IsNullOrEmpty(ttlAsString))
            {
                ttl = 10;
                from = new RavenJArray();
            }
            else
            {
                ttl = int.Parse(ttlAsString);
                from = await ReadJsonArrayAsync().ConfigureAwait(false);
            }

            var replicationSchemaDiscoverer = new ReplicationTopologyDiscoverer(Database, from.Values<string>(), ttl, Log);
            var node = replicationSchemaDiscoverer.Discover();

            return GetMessageWithObject(node);
        }
    }
}
