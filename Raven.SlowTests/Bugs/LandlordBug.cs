// -----------------------------------------------------------------------
//  <copyright file="DatabaseLandlordBug.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.SlowTests.Bugs
{
    public class LandlordBug : RavenTest
    {
        [Fact]
        public void ShouldNotThrowEsentTempPathInUseException()
        {
            using (var store = NewDocumentStore(requestedStorage: "esent", runInMemory: false))
            {
                const string dbName = "TempPathInUse";

                store.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument()
                {
                    Id = "Raven35.Databases/" + dbName,
                    Settings =
                    {
                        {
                            "Raven/DataDir", NewDataPath()
                        }
                    }
                });

                var r = new Random();

                var modifyDocDb = Task.Factory.StartNew(() =>
                {
                    var sw = Stopwatch.StartNew();

                    while (sw.Elapsed < TimeSpan.FromSeconds(30))
                    {
                        try
                        {
                            var jsonDocument = store.SystemDatabase.Documents.Get("Raven35.Databases/" + dbName, null);

                            store.SystemDatabase.Documents.Put("Raven35.Databases/" + dbName, null, jsonDocument.DataAsJson, jsonDocument.Metadata, null);
                        }
                        catch (Exception)
                        {
                            // ignore exceptions
                        }

                        Thread.Sleep(r.Next(50, 100));
                    }
                });

                var tempPathInUseException = new ConcurrentBag<Exception>();

                var accessDb1 = Task.Factory.StartNew(() =>
                {
                    var sw = Stopwatch.StartNew();

                    while (sw.Elapsed < TimeSpan.FromSeconds(30))
                    {
                        try
                        {
                            store.DatabaseCommands.ForDatabase(dbName).GetStatistics();
                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("System.TimeoutException"))
                            {
                                Thread.Sleep(500);
                                continue;
                            }

                            if (e.Message.Contains("EsentTempPathInUseException"))
                                tempPathInUseException.Add(e);
                        }

                        Thread.Sleep(r.Next(10, 20));
                    }
                });

                var accessDb2 = Task.Factory.StartNew(() =>
                {
                    var sw = Stopwatch.StartNew();

                    while (sw.Elapsed < TimeSpan.FromSeconds(30))
                    {
                        try
                        {
                            store.DatabaseCommands.ForDatabase(dbName).GetStatistics();
                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("System.TimeoutException"))
                            {
                                Thread.Sleep(500);
                                continue;
                            }

                            if (e.Message.Contains("EsentTempPathInUseException"))
                                tempPathInUseException.Add(e);
                        }

                        Thread.Sleep(r.Next(15, 30));
                    }
                });

                Task.WaitAll(new[]
                {
                    modifyDocDb, accessDb1, accessDb2
                });

                Assert.True(tempPathInUseException.Count == 0, "Number of EsentTempPathInUseExceptions is " + tempPathInUseException.Count);
            }
        }
    }
}
