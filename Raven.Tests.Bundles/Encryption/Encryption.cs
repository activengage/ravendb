//-----------------------------------------------------------------------
// <copyright file="Expiration.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Client.Document;
using Raven35.Server;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Util;

namespace Raven35.Tests.Bundles.Encryption
{
    public abstract class Encryption : RavenTest
    {
        private readonly string path;
        protected readonly DocumentStore documentStore;
        protected RavenDbServer ravenDbServer;
        private bool closed = false;

        protected Encryption()
        {
            path = NewDataPath();
            CreateServer();
            documentStore = NewRemoteDocumentStore(ravenDbServer: ravenDbServer);
        }

        private void CreateServer()
        {
            ravenDbServer = GetNewServer(
                runInMemory: false, dataDirectory: path, activeBundles: "Encryption",
                configureConfig: configuration =>
            {
                configuration.Settings["Raven/Encryption/Key"] = "3w17MIVIBLSWZpzH0YarqRlR2+yHiv1Zq3TCWXLEMI8=";
            });
        }


        protected void AssertPlainTextIsNotSavedInDatabase(params string[] plaintext)
        {
            Close();
            EncryptionTestUtil.AssertPlainTextIsNotSavedInAnyFileInPath(plaintext, path, s => true);
        }

        protected void RecycleServer()
        {
            ravenDbServer.Dispose();
            CreateServer();
        }

        protected void Close()
        {
            if (closed)
                return;

            documentStore.Dispose();
            ravenDbServer.Dispose();
            closed = true;
        }

        public override void Dispose()
        {
            Close();
            base.Dispose();
        }
    }
}
