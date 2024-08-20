// -----------------------------------------------------------------------
//  <copyright file="Raven_401.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;
using Raven35.Client.Extensions;
using Raven35.Abstractions.Extensions;

namespace Raven35.Tests.Issues
{
    public class RavenDB_410 : RavenTest
    {
        [Fact]
        public void CanCreateDatabaseUsingApi()
        {
            using(GetNewServer())
            using(var store = new DocumentStore
                                {
                                    Url = "http://localhost:8079"
                                }.Initialize())
            {
                store.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument
                {
                    Id = "mydb",
                    Settings =
                    {
                        {"Raven/DataDir", @"~\Databases\Mine"}
                    }
                });

                Assert.DoesNotThrow(() => store.DatabaseCommands.ForDatabase("mydb").Get("test"));
            }
        }

        [Fact]
        public void CanCreateDatabaseWithHiddenData()
        {
            using (GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                store.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument
                                                        {
                                                            Id = "mydb",
                                                            Settings =
                                                                {
                                                                    {"Raven/DataDir", @"~\Databases\Mine"}
                                                                },
                                                            SecuredSettings =
                                                                {
                                                                    {"Secret", "Pass"}
                                                                }
                                                        });

                var jsonDocument = store.DatabaseCommands.Get("Raven35.Databases/mydb");
                var jsonDeserialization = jsonDocument.DataAsJson.JsonDeserialization<DatabaseDocument>();
                Assert.NotEqual("Pass", jsonDeserialization.SecuredSettings["Secret"]);
            }
        }

        [Fact]
        public void TheDatabaseCanReadSecretInfo()
        {
            using (var server = GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                store.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument
                {
                    Id = "mydb",
                    Settings =
                                                                {
                                                                    {"Raven/DataDir", @"~\Databases\Mine"}
                                                                },
                    SecuredSettings =
                                                                {
                                                                    {"Secret", "Pass"}
                                                                }
                });

                var documentDatabase = server.Server.GetDatabaseInternal("mydb");
                documentDatabase.Wait();
                Assert.Equal("Pass", documentDatabase.Result.Configuration.Settings["Secret"]);
            }
        }
    }
}
