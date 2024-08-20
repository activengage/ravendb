// -----------------------------------------------------------------------
//  <copyright file="RDBQA_7.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;

namespace Raven35.Tests.Issues
{
    using System;
    using System.IO;

    using Lucene.Net.Support;

    using Raven35.Abstractions.Data;
    using Raven35.Abstractions.Smuggler;
    using Raven35.Client;
    using Raven35.Database.Extensions;
    using Raven35.Json.Linq;
    using Raven35.Smuggler;

    using Xunit;

    public class RDBQA_7 : RavenTest
    {
        private class Product
        {
            public int Id { get; set; }

            public string Value { get; set; }
        }

        [Fact, Trait("Category", "Smuggler")]
        public void NegativeFiltersShouldNotFilterOutWhenThereAreNoMatches()
        {
            var path = Path.GetTempFileName();

            var options = new SmugglerDatabaseOptions
                {
                    Filters =
                        new EquatableList<FilterSetting>
                        {
                            new FilterSetting
                            {
                                Path = "Value",
                                ShouldMatch = false,
                                Values = new EquatableList<string> {"Value1"}
                            }
                        }
                };

            try
            {
                using (var store = NewRemoteDocumentStore())
                {
                    Initialize(store);

                    var smuggler = new SmugglerDatabaseApi(options);
                    smuggler.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions> { ToFile = path, From = new RavenConnectionStringOptions { Url = store.Url, DefaultDatabase = store.DefaultDatabase } }).Wait(TimeSpan.FromSeconds(15));
                }

                using (var store = NewRemoteDocumentStore())
                {
                    var smuggler = new SmugglerDatabaseApi(options);
                    smuggler.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions> { FromFile = path, To = new RavenConnectionStringOptions { Url = store.Url, DefaultDatabase = store.DefaultDatabase } }).Wait(TimeSpan.FromSeconds(15));

                    Assert.NotNull(store.DatabaseCommands.Get("key/1"));

                    using (var session = store.OpenSession())
                    {
                        var product1 = session.Load<Product>(1);
                        var product2 = session.Load<Product>(2);
                        var product3 = session.Load<Product>(3);

                        Assert.Null(product1);
                        Assert.Null(product2);
                        Assert.NotNull(product3);
                    }
                }
            }
            finally
            {
                IOExtensions.DeleteDirectory(path);
            }
        }

        [Fact, Trait("Category", "Smuggler")]
        public void NegativeMetadataFiltersShouldNotFilterOutWhenThereAreNoMatches()
        {
            var path = Path.GetTempFileName();

            var options = new SmugglerDatabaseOptions
                {
                    Filters =
                        new EquatableList<FilterSetting>
                        {
                            new FilterSetting
                            {
                                Path = "@metadata.Raven-Entity-Name",
                                ShouldMatch = false,
                                Values = new EquatableList<string> {"Products"}
                            }
                        }
                };

            try
            {
                using (var store = NewRemoteDocumentStore())
                {
                    Initialize(store);

                    var smuggler = new SmugglerDatabaseApi(options);
                    smuggler.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions> { ToFile = path, From = new RavenConnectionStringOptions { Url = store.Url, DefaultDatabase = store.DefaultDatabase } }).Wait(TimeSpan.FromSeconds(15));
                }

                using (var store = NewRemoteDocumentStore())
                {
                    var smuggler = new SmugglerDatabaseApi(options);
                    smuggler.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions> { FromFile = path, To = new RavenConnectionStringOptions { Url = store.Url, DefaultDatabase = store.DefaultDatabase } }).Wait(TimeSpan.FromSeconds(15));

                    Assert.NotNull(store.DatabaseCommands.Get("key/1"));

                    using (var session = store.OpenSession())
                    {
                        var product1 = session.Load<Product>(1);
                        var product2 = session.Load<Product>(2);
                        var product3 = session.Load<Product>(3);

                        Assert.Null(product1);
                        Assert.Null(product2);
                        Assert.Null(product3);
                    }
                }
            }
            finally
            {
                IOExtensions.DeleteDirectory(path);
            }
        }

        private static void Initialize(IDocumentStore store)
        {
            store.DatabaseCommands.Put("key/1", null, new RavenJObject(), new RavenJObject());

            var product1 = new Product { Value = "Value1" };
            var product2 = new Product { Value = "Value1" };
            var product3 = new Product { Value = "Value2" };

            using (var session = store.OpenSession())
            {
                session.Store(product1);
                session.Store(product2);
                session.Store(product3);

                session.SaveChanges();
            }
        }
    }
}
