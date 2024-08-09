// -----------------------------------------------------------------------
//  <copyright file="FilesEncryption.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Runtime.CompilerServices;

using Raven35.Client.FileSystem;
using Raven35.Database.Extensions;
using Raven35.Tests.Common.Util;
using Raven35.Tests.Helpers;

namespace Raven35.Tests.FileSystem.Bundles.Encryption
{
    public class FileSystemEncryptionTest : RavenFilesTestBase
    {
        protected readonly string dataPath;

        public FileSystemEncryptionTest()
        {
            dataPath = NewDataPath("RavenFS_Encryption_Test", deleteOnDispose: false);
        }

        protected IAsyncFilesCommands NewAsyncClientForEncryptedFs(string requestedStorage, [CallerMemberName] string fileSystemName = null)
        {
            return NewAsyncClient(requestedStorage: requestedStorage, runInMemory: false, fileSystemName: fileSystemName, dataDirectory: dataPath, activeBundles: "Encryption", customConfig: configuration =>
            {
                configuration.Settings["Raven/Encryption/Key"] = "3w17MIVIBLSWZpzH0YarqRlR2+yHiv1Zq3TCWXLEMI8=";
            });
        }

        protected void Close()
        {
            base.Dispose();
        }

        protected void AssertPlainTextIsNotSavedInFileSystem(params string[] plaintext)
        {
            Close();

            EncryptionTestUtil.AssertPlainTextIsNotSavedInAnyFileInPath(plaintext, dataPath, s => true);
        }

        public override void Dispose()
        {
            Close();

            IOExtensions.DeleteDirectory(dataPath);
        }
    }
}
