// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3917.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Smuggler;
using Raven35.Smuggler;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3917 : RavenTest
    {
        [Fact]
        public async Task SmugglerShouldNotExportImportSubscribtionIdentities()
        {
            using (var store = NewRemoteDocumentStore())
            {
                store.Subscriptions.Create(new SubscriptionCriteria());

                var smuggler = new SmugglerDatabaseApi(new SmugglerDatabaseOptions { OperateOnTypes = ItemType.Documents });

                using (var stream = new MemoryStream())
                {
                    await smuggler.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions>
                    {
                        From = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = store.DefaultDatabase,
                            Url = store.Url
                        },
                        ToStream = stream
                    });

                    stream.Position = 0;

                    store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("Northwind");

                    await smuggler.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions>
                    {
                        FromStream = stream,
                        To = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = "Northwind",
                            Url = store.Url
                        }
                    });
                }
            }
        }
    }
}
