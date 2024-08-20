// -----------------------------------------------------------------------
//  <copyright file="ClusterConfigurationUpdateCommandHandler.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Database.Raft.Commands;
using Raven35.Database.Server.Tenancy;
using Raven35.Json.Linq;

namespace Raven35.Database.Raft.Storage.Handlers
{
    public class ClusterConfigurationUpdateCommandHandler : CommandHandler<ClusterConfigurationUpdateCommand>
    {
        public ClusterConfigurationUpdateCommandHandler(DocumentDatabase database, DatabasesLandlord landlord)
            : base(database, landlord)
        {
        }

        public override void Handle(ClusterConfigurationUpdateCommand command)
        {
            Database.Documents.Put(Constants.Cluster.ClusterConfigurationDocumentKey, null, RavenJObject.FromObject(command.Configuration), new RavenJObject(), null);
        }
    }
}
