// -----------------------------------------------------------------------
//  <copyright file="DbShutDownMidTransaction.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using Raven35.Abstractions.Data;
using Raven35.Database.Extensions;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.DTC.NSB
{
    public class DbShutDownMidTransaction : RavenTest
    {
        public DbShutDownMidTransaction()
        {
            IOExtensions.DeleteDirectory("DbShutDownMidTransaction");
        }

        public override void Dispose()
        {
            base.Dispose();
            IOExtensions.DeleteDirectory("DbShutDownMidTransaction");
        }

        [Fact]
        public void WillAllowCommittingTransactionFromBeforeShutdown()
        {
            using (var store = NewDocumentStore(runInMemory: false, requestedStorage: "esent", dataDir: "DbShutDownMidTransaction"))
            {
                var tx = new TransactionInformation
                {
                    Id = "tx",
                    Timeout = TimeSpan.FromHours(1)
                };

                store.SystemDatabase.Documents.Put("test", null, new RavenJObject(), new RavenJObject(), tx);

                store.SystemDatabase.PrepareTransaction("tx");
            }

            using ( var store = NewDocumentStore(runInMemory: false, requestedStorage: "esent", dataDir: "DbShutDownMidTransaction"))
            {
                store.SystemDatabase.Commit("tx");


                Assert.NotNull(store.SystemDatabase.Documents.Get("test", null));
            }
        }
    }
}
