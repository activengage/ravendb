//-----------------------------------------------------------------------
// <copyright file="OperationHeaders.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition.Hosting;
using Raven35.Abstractions.Data;
using Raven35.Client.Document;
using Raven35.Database.Plugins;
using Raven35.Database.Server;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class OperationHeaders : RavenTest
    {
        [Fact]
        public void CanPassOperationHeadersUsingEmbedded()
        {
            using (var documentStore = NewDocumentStore(configureStore: store => store.Configuration.Catalog.Catalogs.Add(new TypeCatalog(typeof (RecordOperationHeaders)))))
            {
                RecordOperationHeaders.Hello = null;
                using(var session = documentStore.OpenSession())
                {
                    ((DocumentSession)session).DatabaseCommands.OperationsHeaders["Hello"] = "World";
                    session.Store(new { Bar = "foo"});
                    session.SaveChanges();

                    Assert.Equal("World", RecordOperationHeaders.Hello);
                }
            }
        }

        public class RecordOperationHeaders : AbstractPutTrigger
        {
            public static string Hello;

            public override void OnPut(string key, RavenJObject jsonReplicationDocument, RavenJObject metadata, TransactionInformation transactionInformation)
            {
                Hello = CurrentOperationContext.Headers.Value.Value["Hello"];
                base.OnPut(key, jsonReplicationDocument, metadata, transactionInformation);
            }
        }
    }
}
