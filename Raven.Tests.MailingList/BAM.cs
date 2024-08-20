using System;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;
using Raven35.Client.Extensions;

namespace Raven35.Tests.MailingList
{
    public class BAM : RavenTest
    {
        [Fact]
        public void get_dbnames_test()
        {
            using (var server = GetNewServer(databaseName: Constants.SystemDatabase))
            using (var docStore = new DocumentStore {Url = "http://localhost:8079"}.Initialize())
            {
                var dbNames = docStore.DatabaseCommands.GlobalAdmin.GetDatabaseNames(25, 0);

                Assert.Empty(dbNames);

                docStore.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("test");

                dbNames = docStore.DatabaseCommands.GlobalAdmin.GetDatabaseNames(25, 0);

                Assert.NotEmpty(dbNames);

            }
        }



        [Fact]
        public void id_with_backslash_remote()
        {
            var goodId = "good/one";
            var badId = @"bad\one";

            using(GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    var goodIn = new {Id = goodId};
                    session.Store(goodIn);

                    var badIn = new {Id = badId};
                    session.Store(badIn);

                    var throws = Assert.Throws<ErrorResponseException>(()=>session.SaveChanges());

                    Assert.Contains(@"PUT vetoed on document bad\one by Raven35.Database.Plugins.Builtins.InvalidDocumentNames because: Document name cannot contain '\' but attempted to save with: bad\one", throws.Message);
                }
            }
        }



        [Fact]
        public void Cannot_create_tenant_named_system()
        {
            using (GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                var throws = Assert.Throws<ErrorResponseException>(() => store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("System"));

                Assert.Contains(@"Cannot create a tenant database with the name 'System', that name is reserved for the actual system database", throws.Message);
        
            }
        }
    }
}
