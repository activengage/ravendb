// -----------------------------------------------------------------------
//  <copyright file="BulkInsertAuth.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Connection;
using Raven35.Client.Connection.Async;
using Raven35.Client.Document;
using Raven35.Database.Server;
using Raven35.Database.Server.Security;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class BulkInsertAuth : RavenTest
    {
        protected override void ModifyConfiguration(Database.Config.InMemoryRavenConfiguration configuration)
        {
            configuration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            Authentication.EnableOnce();
        }

        [Fact]
        public void CanBulkInsertWithWindowsAuth()
        {
            using (var store = NewRemoteDocumentStore())
            {
                using (var op = new RemoteBulkInsertOperation(new BulkInsertOptions(), (AsyncServerClient)store.AsyncDatabaseCommands, store.Changes()))
                {
                    op.Write("items/1", new RavenJObject(), new RavenJObject());
                }
                Assert.NotNull(store.DatabaseCommands.Get("items/1"));
            }
        }
    }

    public class BulkInsertOAuth : RavenTest
    {
        protected override void ModifyConfiguration(Database.Config.InMemoryRavenConfiguration configuration)
        {
            configuration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            Authentication.EnableOnce();
        }

        protected override void ModifyServer(Server.RavenDbServer ravenDbServer)
        {
            var id = "Raven/ApiKeys/test";
            ravenDbServer.SystemDatabase.Documents.Put(id, null,
                                       RavenJObject.FromObject(new ApiKeyDefinition
                                       {
                                           Id = id,
                                           Name = "test",
                                           Secret = "test",
                                           Enabled = true,
                                           Databases =
                                           {
                                               new ResourceAccess {Admin = true, TenantId = "*"},
                                               new ResourceAccess {Admin = true, TenantId = "<system>"}
                                           }
                                       }), new RavenJObject(), null);
        }

        protected override void ModifyStore(DocumentStore documentStore)
        {
            documentStore.ApiKey = "test/test";
            documentStore.Conventions.FailoverBehavior = FailoverBehavior.FailImmediately;
        }

        [Fact]
        public void CanBulkInsertWithApiKey()
        {
            using (var store = NewRemoteDocumentStore(enableAuthentication: true))
            {
                using (var op = new RemoteBulkInsertOperation(new BulkInsertOptions(), (AsyncServerClient)store.AsyncDatabaseCommands, store.Changes()))
                {
                    op.Write("items/1", new RavenJObject(), new RavenJObject());
                }
                Assert.NotNull(store.DatabaseCommands.Get("items/1"));
            }
        }
    }
}
