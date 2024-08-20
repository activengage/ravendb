// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3658.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Smuggler;
using Raven35.Smuggler;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;
using Raven35.Tests.Core.Smuggler;

using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3658 : ReplicationBase
    {
        [Theory]
        [InlineData("voron")]
        [InlineData("esent")]
        public async Task ShouldImportHiloWhenRavenEntityNameFilterIsUsed(string storage)
        {
            using (var store1 = CreateStore(requestedStorageType:storage))
            using (var store2 = CreateStore(requestedStorageType:storage))
            {
                using (var session = store1.OpenSession())
                {
                    session.Store(new Person { Name = "John" });
                    session.Store(new Person { Name = "Edward" });
                    session.Store(new Address { Street = "Main Street" });

                    session.SaveChanges();
                }

                using (var stream = new MemoryStream())
                {
                    var smugglerApi = new SmugglerDatabaseApi(new SmugglerDatabaseOptions
                    {
                        Filters =
                        {
                            new FilterSetting
                            {
                                Path = "@metadata.Raven-Entity-Name",
                                ShouldMatch = true,
                                Values = { "People" }
                            }
                        }
                    });

                    await smugglerApi.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions>
                    {
                        From = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = store1.DefaultDatabase,
                            Url = store1.Url
                        },
                        ToStream = stream
                    });

                    stream.Position = 0;

                    await smugglerApi.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions>
                    {
                        FromStream = stream,
                        To = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = store2.DefaultDatabase,
                            Url = store2.Url
                        }
                    });
                }

                WaitForIndexing(store2);

                using (var session = store2.OpenSession())
                {
                    var people = session
                        .Query<Person>()
                        .ToList();

                    Assert.Equal(2, people.Count);

                    var addresses = session
                        .Query<Address>()
                        .ToList();

                    Assert.Equal(0, addresses.Count);

                    var hilo = session.Advanced.DocumentStore.DatabaseCommands.Get("Raven/Hilo/People");
                    Assert.NotNull(hilo);
                }
            }
        }

        [Theory]
        [InlineData("voron")]
        [InlineData("esent")]
        public async Task ShouldImportHiloWhenRavenEntityNameFilterIsUsed_Between(string storage)
        {
            using (var store1 = CreateStore(requestedStorageType:storage))
            using (var store2 = CreateStore(requestedStorageType:storage))
            {
                using (var session = store1.OpenSession())
                {
                    session.Store(new Person { Name = "John" });
                    session.Store(new Person { Name = "Edward" });
                    session.Store(new Address { Street = "Main Street" });

                    session.SaveChanges();
                }

                using (var stream = new MemoryStream())
                {
                    var smugglerApi = new SmugglerDatabaseApi(new SmugglerDatabaseOptions
                    {
                        Filters =
                        {
                            new FilterSetting
                            {
                                Path = "@metadata.Raven-Entity-Name",
                                ShouldMatch = true,
                                Values = { "People" }
                            }
                        }
                    });

                    await smugglerApi.Between(new SmugglerBetweenOptions<RavenConnectionStringOptions>
                    {
                        From = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = store1.DefaultDatabase,
                            Url = store1.Url
                        },
                        To = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = store2.DefaultDatabase,
                            Url = store2.Url
                        }
                    });
                }

                WaitForIndexing(store2);

                using (var session = store2.OpenSession())
                {
                    var people = session
                        .Query<Person>()
                        .ToList();

                    Assert.Equal(2, people.Count);

                    var addresses = session
                        .Query<Address>()
                        .ToList();

                    Assert.Equal(0, addresses.Count);

                    var hilo = session.Advanced.DocumentStore.DatabaseCommands.Get("Raven/Hilo/People");
                    Assert.NotNull(hilo);
                }
            }
        }
    }
}
