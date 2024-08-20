//-----------------------------------------------------------------------
// <copyright file="Compression.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using Raven35.Client.Document;
using Raven35.Server;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Util;

namespace Raven35.Tests.Bundles.Compression
{
    public abstract class Compression : RavenTest
    {
        private readonly string path;
        private readonly RavenDbServer ravenDbServer;
        protected readonly DocumentStore documentStore;

        protected Compression()
        {
            // This will be disposed by the RavenTestBase.Dispose method
            path = NewDataPath("Compression");
            ravenDbServer = GetNewServer(activeBundles: "Compression", dataDirectory: path, runInMemory:false);
            documentStore = NewRemoteDocumentStore(ravenDbServer: ravenDbServer);
        }

        protected void AssertPlainTextIsNotSavedInDatabase_ExceptIndexes(params string[] plaintext)
        {
            documentStore.Dispose();
            ravenDbServer.Dispose();
            EncryptionTestUtil.AssertPlainTextIsNotSavedInAnyFileInPath(plaintext, path, file => Path.GetExtension(file) != ".cfs");
        }
    }
}
