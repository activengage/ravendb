// -----------------------------------------------------------------------
//  <copyright file="ApiKeysWithMultiTenancy.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Document;
using Raven35.Database.Config;
using Raven35.Database.Server;
using Raven35.Database.Server.Security;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;
using Raven35.Client.Extensions;

namespace Raven35.Tests.Bundles.Replication.Bugs
{
    public class ApiKeysWithMultiTenancy : ReplicationBase
    {
        private const string apikey = "test/ThisIsMySecret";

        protected override void ModifyConfiguration(InMemoryRavenConfiguration serverConfiguration)
        {
            serverConfiguration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            Authentication.EnableOnce();
        }

        protected override void ConfigureDatabase(Database.DocumentDatabase database, string databaseName = null)
        {
            database.Documents.Put("Raven/ApiKeys/test", null, RavenJObject.FromObject(new ApiKeyDefinition
            {
                Name = "test",
                Secret = "ThisIsMySecret",
                Enabled = true,
                Databases = new List<ResourceAccess>
                {
                    new ResourceAccess {TenantId = "*", Admin = true},
                    new ResourceAccess {TenantId = Constants.SystemDatabase, Admin = true},
                    new ResourceAccess {TenantId = databaseName, Admin = true}
                }
            }), new RavenJObject(), null);
        }

        [Fact]
        public void CanReplicationToChildDbsUsingApiKeys()
        {
            var store1 = CreateStore(configureStore: store =>
            {
                store.ApiKey = apikey;
                store.Conventions.FailoverBehavior=FailoverBehavior.FailImmediately;
            },enableAuthorization: true);
            var store2 = CreateStore(configureStore: store =>
            {
                store.ApiKey = apikey;
                store.Conventions.FailoverBehavior = FailoverBehavior.FailImmediately;
            }, enableAuthorization: true);

            store1.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument
            {
                Id = "repl",
                Settings =
                {
                    {Constants.RunInMemory, "true"},
                    {"Raven/DataDir", "~/db1"},
                    {"Raven/ActiveBundles", "Replication"}
                }
            });
            store2.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument
            {
                Id = "repl",
                Settings =
                {
                    {Constants.RunInMemory, "true"},
                    {"Raven/DataDir", "~/db2"},
                    {"Raven/ActiveBundles", "Replication"}
                }
            });

            RunReplication(store1, store2, apiKey: apikey, db: "repl");

            using (var s = store1.OpenSession("repl"))
            {
                s.Store(new User());
                s.SaveChanges();
            }

            WaitForReplication(store2, "users/1", db: "repl");
        }
    }
}
