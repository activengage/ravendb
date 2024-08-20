using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Util;
using Rachis;
using Raven35.Abstractions.Extensions;
using Raven35.Bundles.Replication.Data;
using Raven35.Database.Raft.Commands;
using Raven35.Database.Server.Tenancy;
using Raven35.Json.Linq;

namespace Raven35.Database.Raft.Storage.Handlers
{
    public class ReplicationStateCommandHandler : CommandHandler<ReplicationStateCommand>
    {
        public ReplicationStateCommandHandler(DocumentDatabase database, DatabasesLandlord landlord) : base(database, landlord)
        {

        }


        public override void Handle(ReplicationStateCommand command)
        {
            var key = Abstractions.Data.Constants.Cluster.ClusterReplicationStateDocumentKey;
            Database.Documents.Put(key, null, RavenJObject.FromObject(command.DatabaseToLastModified), new RavenJObject(), null);
        }
    }
}
