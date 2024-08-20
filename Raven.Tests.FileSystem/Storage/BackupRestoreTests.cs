// -----------------------------------------------------------------------
//  <copyright file="BackupRestoreTests.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Client.FileSystem;
using Raven35.Database.Extensions;
using Raven35.Tests.FileSystem.Synchronization.IO;

using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests.FileSystem.Storage
{
    /// <summary>
    /// RavenDB-2699
    /// </summary>
    public class BackupRestoreTests : RavenFilesTestWithLogs
    {
        private readonly string DataDir;
        private readonly string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupRestoreTests.Backup");

        public BackupRestoreTests()
        {
            DataDir = NewDataPath("DataDirectory");
            IOExtensions.DeleteDirectory(backupDir);
        }

        public override void Dispose()
        {
            IOExtensions.DeleteDirectory(backupDir);
            base.Dispose();
        }

        [Theory]
        [PropertyData("Storages")]
        public async Task CanRestoreBackupToDifferentFilesystem(string requestedStorage)
        {
            using (var store = (FilesStore)NewStore(requestedStorage: requestedStorage, runInMemory: false))
            {
                var server = this.GetServer();

                string filesystemDir = Path.Combine(server.Configuration.FileSystem.DataDirectory, "NewFS");

                await CreateSampleData(store);

                // fetch md5 sums for later verification
                var md5Sums = FetchMd5Sums(store.AsyncFilesCommands);

                // create backup
                var opId = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, false, store.DefaultFileSystem);
                await WaitForOperationAsync(store.Url, opId);

                // restore newly created backup
                await store.AsyncFilesCommands.Admin.StartRestore(new FilesystemRestoreRequest
                {
                    BackupLocation = backupDir,
                    FilesystemName = "NewFS",
                    FilesystemLocation = filesystemDir
                });

                SpinWait.SpinUntil(() => store.AsyncFilesCommands.Admin.GetNamesAsync().Result.Contains("NewFS"),
                            Debugger.IsAttached ? TimeSpan.FromMinutes(10) : TimeSpan.FromMinutes(1));

                var restoredMd5Sums = FetchMd5Sums(store.AsyncFilesCommands.ForFileSystem("NewFS"));
                Assert.Equal(md5Sums, restoredMd5Sums);

                var restoredClientComputedMd5Sums = ComputeMd5Sums(store.AsyncFilesCommands.ForFileSystem("NewFS"));
                Assert.Equal(md5Sums, restoredClientComputedMd5Sums);
            }
        }

        [Theory]
        [PropertyData("Storages")]
        public async Task CanRestoreIncrementalBackupToDifferentFilesystem(string requestedStorage)
        {
            using (var store = NewStore(requestedStorage: requestedStorage, runInMemory: false, customConfig:config =>
            {
                config.Settings["Raven/Esent/CircularLog"] = "false";
                config.Settings["Raven35.Voron/AllowIncrementalBackups"] = "true";
                config.Storage.Voron.AllowIncrementalBackups = true;
            }, fileSystemName: "FS1"))
            {
                await CreateSampleData(store);
                // create backup
                var opId = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, true, store.DefaultFileSystem);
                await WaitForOperationAsync(store.Url, opId);

                await CreateSampleData(store, 3, 5);
                
                // fetch md5 sums for later verification
                var md5Sums = FetchMd5Sums(store.AsyncFilesCommands, 7);

                // create second backup
                var opId2 = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, true, store.DefaultFileSystem);
                await WaitForOperationAsync(store.Url, opId2);

                // restore newly created backup
                await store.AsyncFilesCommands.Admin.StartRestore(new FilesystemRestoreRequest
                                                                  {
                                                                      BackupLocation = backupDir,
                                                                      FilesystemName = "NewFS",
                                                                      FilesystemLocation = DataDir
                                                                  });

                SpinWait.SpinUntil(() => store.AsyncFilesCommands.Admin.GetNamesAsync().Result.Contains("NewFS"),
                    Debugger.IsAttached ? TimeSpan.FromMinutes(120) : TimeSpan.FromMinutes(1));

                var restoredMd5Sums = FetchMd5Sums(store.AsyncFilesCommands.ForFileSystem("NewFS"), 7);
                Assert.Equal(md5Sums, restoredMd5Sums);

                var restoredClientComputedMd5Sums = ComputeMd5Sums(store.AsyncFilesCommands.ForFileSystem("NewFS"), 7);
                Assert.Equal(md5Sums, restoredClientComputedMd5Sums);
            }

        }

        [Theory]
        [PropertyData("Storages")]
        public async Task RavenDB_2824_ShouldThrowWhenTryingToUseTheSameIncrementalBackupLocationForDifferentFS(string requestedStorage)
        {
            using (var store = (FilesStore)NewStore(requestedStorage: requestedStorage, runInMemory: false, customConfig: config =>
            {
                config.Settings["Raven/Esent/CircularLog"] = "false";
                config.Settings["Raven35.Voron/AllowIncrementalBackups"] = "true";
                config.Storage.Voron.AllowIncrementalBackups = true;
            }, fileSystemName: "RavenDB_2824_one"))
            {
                await CreateSampleData(store);
                
                var opId1 = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, true, store.DefaultFileSystem);
                await WaitForOperationAsync(store.Url, opId1);

                await CreateSampleData(store, 3, 5);

                var opId2 = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, true, store.DefaultFileSystem);
                await WaitForOperationAsync(store.Url, opId2);
            }

            using (var store = (FilesStore)NewStore(index: 1, requestedStorage: requestedStorage, runInMemory: false, customConfig: config =>
            {
                config.Settings["Raven/Esent/CircularLog"] = "false";
                config.Settings["Raven35.Voron/AllowIncrementalBackups"] = "true";
                config.Storage.Voron.AllowIncrementalBackups = true;
            }, fileSystemName: "RavenDB_2824_two"))
            {
                var opId = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, true, store.DefaultFileSystem);  // use the same BackupDir on purpose

                var ex = Assert.Throws<AggregateException>( () => WaitForOperationAsync(store.Url, opId).Wait());
                var innerEx = ex.ExtractSingleInnerException();

                Assert.Contains("Can't perform an incremental backup to a given folder because it already contains incremental backup data of different file system. Existing incremental data origins from 'RavenDB_2824_one' file system.", innerEx.Message);
            }
        }

        [Theory]
        [PropertyData("Storages")]
        public async Task RavenDB_2950_EvenIfTheIndexIsCorruptedItShouldDisposeCorrectly(string requestedStorage)
        {
            using (var store = (FilesStore)NewStore(requestedStorage: requestedStorage, runInMemory: false))
            {
                var server = this.GetServer();

                string filesystemDir = Path.Combine(server.Configuration.FileSystem.DataDirectory, "NewFS");

                await CreateSampleData(store);

                // fetch md5 sums for later verification
                var md5Sums = FetchMd5Sums(store.AsyncFilesCommands);

                // create backup
                var opId = await store.AsyncFilesCommands.Admin.StartBackup(backupDir, null, false, store.DefaultFileSystem);
                await WaitForOperationAsync(store.Url, opId);

                // restore newly created backup
                await store.AsyncFilesCommands.Admin.StartRestore(new FilesystemRestoreRequest
                {
                    BackupLocation = backupDir,
                    FilesystemName = "NewFS",
                    FilesystemLocation = filesystemDir
                });

                SpinWait.SpinUntil(() => store.AsyncFilesCommands.Admin.GetNamesAsync().Result.Contains("NewFS"),
                            Debugger.IsAttached ? TimeSpan.FromMinutes(10) : TimeSpan.FromMinutes(1));

                // Corrupt the index on purpose.
                File.Delete(Path.Combine(filesystemDir, "Indexes", "index.version"));

                var restoredMd5Sums = FetchMd5Sums(store.AsyncFilesCommands.ForFileSystem("NewFS"));
                Assert.Equal(md5Sums, restoredMd5Sums);

                var restoredClientComputedMd5Sums = ComputeMd5Sums(store.AsyncFilesCommands.ForFileSystem("NewFS"));
                Assert.Equal(md5Sums, restoredClientComputedMd5Sums);
            }
        }

        private string[] ComputeMd5Sums(IAsyncFilesCommands filesCommands, int filesCount = 2)
        {
            return Enumerable.Range(1, filesCount).Select(i =>
            {
                using (var stream = filesCommands.DownloadAsync(string.Format("file{0}.bin", i)).Result)
                {
                    return stream.GetMD5Hash();
                }
            }).ToArray();
        }

        private async Task CreateSampleData(IFilesStore filesStore, int startIndex = 1 , int count = 2)
        {
            for (var i = startIndex; i < startIndex + count; i++)
            {
                await filesStore.AsyncFilesCommands.UploadAsync(string.Format("file{0}.bin", i), new RandomStream(10 * i));
            }
        }

        private string[] FetchMd5Sums(IAsyncFilesCommands filesCommands, int filesCount = 2)
        {
            return Enumerable.Range(1, filesCount).Select(i =>
            {
                var meta = filesCommands.GetMetadataForAsync(string.Format("file{0}.bin", i)).Result;
                return meta.Value<string>("Content-MD5");
            }).ToArray();
        }

    }
}
