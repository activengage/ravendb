// -----------------------------------------------------------------------
//  <copyright file="DatabaseDeletedCommandHandler.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Database.Raft.Commands;
using Raven35.Database.Raft.Util;
using Raven35.Database.Server.Tenancy;
using Raven35.Database.Util;

namespace Raven35.Database.Raft.Storage.Handlers
{
    public class DatabaseDeletedCommandHandler : CommandHandler<DatabaseDeletedCommand>
    {
        public DatabaseDeletedCommandHandler(DocumentDatabase database, DatabasesLandlord landlord)
            : base(database, landlord)
        {
        }

        public override void Handle(DatabaseDeletedCommand command)
        {
            var key = DatabaseHelper.GetDatabaseKey(command.Name);

            var documentJson = Database.Documents.Get(key, null);
            if (documentJson == null)
                return;

            var document = documentJson.DataAsJson.JsonDeserialization<DatabaseDocument>();
            if (document.IsClusterDatabase() == false)
                return; // ignore non-cluster databases

            var databaseName = DatabaseHelper.GetDatabaseName(command.Name);
            var configuration = Landlord.CreateTenantConfiguration(databaseName, true);
            var isLoaded = Landlord.IsDatabaseLoaded(databaseName);
            if (isLoaded)
                Landlord.Cleanup(databaseName,null);

            if (configuration == null)
                return;

            Database.Documents.Delete(key, null, null);

            if (command.HardDelete)
                DatabaseHelper.DeleteDatabaseFiles(configuration);
        }
    }
}
