// -----------------------------------------------------------------------
//  <copyright file="DatabaseUpdateCommandHandler.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.Logging;
using Raven35.Database.Raft.Commands;
using Raven35.Database.Raft.Util;
using Raven35.Database.Server.Tenancy;
using Raven35.Database.Util;
using Raven35.Json.Linq;

namespace Raven35.Database.Raft.Storage.Handlers
{
    public class DatabaseUpdateCommandHandler : CommandHandler<DatabaseUpdateCommand>
    {
        private readonly ILog log = LogManager.GetCurrentClassLogger();

        public DatabaseUpdateCommandHandler(DocumentDatabase database, DatabasesLandlord landlord)
            : base(database, landlord)
        {
        }

        public override void Handle(DatabaseUpdateCommand command)
        {
            command.Document.AssertClusterDatabase();

            var key = DatabaseHelper.GetDatabaseKey(command.Document.Id);

            var documentJson = Database.Documents.Get(key, null);
            if (documentJson != null)
            {
                var document = documentJson.DataAsJson.JsonDeserialization<DatabaseDocument>();
                if (document.IsClusterDatabase() == false)
                {
                    log.Error(string.Format("Local database '{0}' is not cluster-wide.", DatabaseHelper.GetDatabaseName(command.Document.Id)));
                    return;
                }
            }

            Landlord.Protect(command.Document);
            var json = RavenJObject.FromObject(command.Document);
            json.Remove("Id");

            try
            {
                Database.Documents.Put(key, null, json, new RavenJObject(), null);
            }
            catch (OperationVetoedException e)
            {
                log.ErrorException("Was not able to update resorce due to veto", e);
            }
        }
    }
}
