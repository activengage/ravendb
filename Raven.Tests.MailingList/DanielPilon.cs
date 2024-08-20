// -----------------------------------------------------------------------
//  <copyright file="DanielPilon.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Client.Document;
using Raven35.Client.Embedded;
using Raven35.Client.Extensions;
using Raven35.Client.Indexes;
using Xunit;

namespace Raven35.Tests.MailingList
{
    public class DanielPilon
    {
        [Fact]
        public void CanShutdown()
        {
            DocumentStore docStore;
            using (var store = new EmbeddableDocumentStore { UseEmbeddedHttpServer = true, RunInMemory = true })
            {
                store.Configuration.Port = 8079;

                store.Initialize();

                docStore = new DocumentStore
                {
                    Url = "http://127.0.0.1:8079/",
                    DefaultDatabase = "test"
                };
                docStore.Initialize();
                new RavenDocumentsByEntityName().Execute(docStore);

                docStore.DatabaseCommands.EnsureDatabaseExists("database");

                using (docStore.OpenSession("database"))
                {
                }
            }
            docStore.Dispose();
        }
    }
}
