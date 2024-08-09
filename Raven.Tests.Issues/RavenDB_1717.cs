// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1717.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IO;

using Raven35.Tests.Common;
using Voron;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_1717 : TransactionalStorageTestBase
    {
        private readonly string path;

        private readonly string temp;

        public RavenDB_1717()
        {
            path = NewDataPath("Data");
            temp = NewDataPath("Temp");
        }

        [Fact]
        public void TempPathForVoronShouldWork1()
        {
            using (var storage = NewTransactionalStorage(requestedStorage: "voron", dataDir: path, runInMemory:false))
            {
                var scratchFile = Path.Combine(path, StorageEnvironmentOptions.ScratchBufferName(0));
                var scratchFileTemp = Path.Combine(temp, StorageEnvironmentOptions.ScratchBufferName(0));

                Assert.True(File.Exists(scratchFile));
                Assert.False(File.Exists(scratchFileTemp));
            }
        }

        [Fact]
        public void TempPathForVoronShouldWork2()
        {
            using (var storage = NewTransactionalStorage(requestedStorage: "voron", dataDir: path, tempDir: temp, runInMemory: false))
            {
                var scratchFile = Path.Combine(path, StorageEnvironmentOptions.ScratchBufferName(0));
                var scratchFileTemp = Path.Combine(temp, StorageEnvironmentOptions.ScratchBufferName(0));

                Assert.False(File.Exists(scratchFile));
                Assert.True(File.Exists(scratchFileTemp));
            }
        }
    }
}
