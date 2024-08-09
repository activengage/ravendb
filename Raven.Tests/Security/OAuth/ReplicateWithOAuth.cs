using System.Collections.Generic;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Document;
using Raven35.Database.Config;
using Raven35.Database.Server;
using Raven35.Database.Server.Security;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Document;
using Xunit;

namespace Raven35.Tests.Security.OAuth
{
    public class ReplicateWithOAuth : ReplicationBase
    {
        private const string apiKey = "test/ThisIsMySecret";

        protected override void ModifyStore(DocumentStore store)
        {
            store.Conventions.FailoverBehavior = FailoverBehavior.FailImmediately;
            store.Credentials = null;
            store.ApiKey = apiKey;
        }

        protected override void ConfigureConfig(InMemoryRavenConfiguration inMemoryRavenConfiguration)
        {
            inMemoryRavenConfiguration.Settings["Raven/Licensing/AllowAdminAnonymousAccessForCommercialUse"] = "true";
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
                    new ResourceAccess {TenantId = "*"},
                    new ResourceAccess {TenantId = Constants.SystemDatabase},
                    new ResourceAccess {TenantId = databaseName, Admin = true}
                }
            }), new RavenJObject(), null);
        }

        [Fact]
        public void Can_Replicate_With_OAuth()
        {
            var store1 = CreateStore(enableAuthorization: true);
            Authentication.EnableOnce();
            var store2 = CreateStore(anonymousUserAccessMode: AnonymousUserAccessMode.None, enableAuthorization: true);
            
            TellFirstInstanceToReplicateToSecondInstance(apiKey);

            using (var session = store1.OpenSession())
            {
                session.Store(new Company { Name = "Hibernating Rhinos" });
                session.SaveChanges();
            }

            var company = WaitForDocument<Company>(store2, "companies/1");
            Assert.Equal("Hibernating Rhinos", company.Name);
        }
    }
}
