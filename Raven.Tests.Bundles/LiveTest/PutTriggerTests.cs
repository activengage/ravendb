// -----------------------------------------------------------------------
//  <copyright file="PutTriggerTests.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.Composition.Hosting;
using System.Configuration;

using Raven35.Abstractions.Data;
using Raven35.Bundles.LiveTest;
using Raven35.Client.Connection;
using Raven35.Database.Config;
using Raven35.Database.Extensions;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bundles.LiveTest
{
    public class PutTriggerTests : RavenTest
    {
        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            configuration.Catalog.Catalogs.Add(new AssemblyCatalog(typeof(LiveTestDatabaseDocumentPutTrigger).Assembly));
        }

        [Fact]
        public void PutTriggerShouldEnableQuotasAndVoron()
        {
            using (var store = NewDocumentStore())
            {
                store
                    .DatabaseCommands
                    .GlobalAdmin
                    .CreateDatabase(new DatabaseDocument
                    {
                        Id = "Northwind",
                        Settings =
                        {
                            { "Raven/ActiveBundles", "Replication" },
                            { "Raven/DataDir", NewDataPath() }
                        }
                    });

                var document = store.DatabaseCommands.Get("Raven35.Databases/Northwind");
                Assert.NotNull(document);

                var databaseDocument = document.DataAsJson.Deserialize<DatabaseDocument>(store.Conventions);
                AsserDatabaseDocument(databaseDocument);

                databaseDocument.Settings[Constants.SizeHardLimitInKB] = "123";
                databaseDocument.Settings[Constants.SizeSoftLimitInKB] = "321";
                databaseDocument.Settings[Constants.DocsHardLimit] = "456";
                databaseDocument.Settings[Constants.DocsSoftLimit] = "654";
                databaseDocument.Settings[Constants.RunInMemory] = "false";
                databaseDocument.Settings["Raven/StorageEngine"] = "esent";

                store.DatabaseCommands.Put("Raven35.Databases/Northwind", null, RavenJObject.FromObject(databaseDocument), document.Metadata);

                document = store.DatabaseCommands.Get("Raven35.Databases/Northwind");
                Assert.NotNull(document);

                databaseDocument = document.DataAsJson.Deserialize<DatabaseDocument>(store.Conventions);
                AsserDatabaseDocument(databaseDocument);
            }
        }

        private static void AsserDatabaseDocument(DatabaseDocument databaseDocument)
        {
            var activeBundles = databaseDocument.Settings[Constants.ActiveBundles].GetSemicolonSeparatedValues();

            Assert.Contains("Replication", activeBundles);

            Assert.Contains("Quotas", activeBundles);
            Assert.Equal(ConfigurationManager.AppSettings["Raven35.Bundles/LiveTest/Quotas/Size/HardLimitInKB"], databaseDocument.Settings[Constants.SizeHardLimitInKB]);
            Assert.Equal(ConfigurationManager.AppSettings["Raven35.Bundles/LiveTest/Quotas/Size/SoftLimitInKB"], databaseDocument.Settings[Constants.SizeSoftLimitInKB]);
            Assert.Null(databaseDocument.Settings[Constants.DocsHardLimit]);
            Assert.Null(databaseDocument.Settings[Constants.DocsSoftLimit]);

            Assert.True(bool.Parse(databaseDocument.Settings[Constants.RunInMemory]));
            Assert.Equal(InMemoryRavenConfiguration.VoronTypeName, databaseDocument.Settings["Raven/StorageEngine"]);
        }
    }
}
