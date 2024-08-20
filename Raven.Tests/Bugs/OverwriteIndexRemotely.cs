//-----------------------------------------------------------------------
// <copyright file="OverwriteIndexRemotely.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Database.Extensions;
using Raven35.Database.Server;
using Raven35.Server;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class OverwriteIndexRemotely : RavenTest, IDisposable
    {
        private readonly RavenDbServer ravenDbServer;
        private readonly IDocumentStore documentStore;

        public OverwriteIndexRemotely()
        {
            const int port = 8079;
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8079);

            ravenDbServer = GetNewServer(port);
            documentStore = new DocumentStore {Url = "http://localhost:" + port}.Initialize();
        }

        public override void Dispose()
        {
            documentStore.Dispose();
            ravenDbServer.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanOverwriteIndex()
        {
            documentStore.DatabaseCommands.PutIndex("test",
                                                    new IndexDefinition
                                                        {
                                                            Map = "from doc in docs select new { doc.Name }"
                                                        }, overwrite: true);


            documentStore.DatabaseCommands.PutIndex("test",
                                                    new IndexDefinition
                                                        {
                                                            Map = "from doc in docs select new { doc.Name }"
                                                        }, overwrite: true);

            documentStore.DatabaseCommands.PutIndex("test",
                                                    new IndexDefinition
                                                        {
                                                            Map = "from doc in docs select new { doc.Email }"
                                                        }, overwrite: true);

            documentStore.DatabaseCommands.PutIndex("test",
                                                    new IndexDefinition
                                                        {
                                                            Map = "from doc in docs select new { doc.Email }"
                                                        }, overwrite: true);
        }
    }
}
