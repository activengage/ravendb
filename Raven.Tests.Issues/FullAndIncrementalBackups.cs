using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Database.Config;
using Raven35.Database.Extensions;
using Raven35.Database.Storage.Voron.Backup;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class FullAndIncrementalBackups : RavenTestBase
    {
        private readonly string BackupFolder = "Backup_" + Guid.NewGuid();

        public FullAndIncrementalBackups()
        {
            //just a precaution
            var fullBackupFolders = Directory.GetDirectories(".\\", "Backup*");
            fullBackupFolders.ForEach(IOExtensions.DeleteDirectory);

        }

        public class User
        {
            public string Name { get; set; }
        }

        protected override void ModifyConfiguration(InMemoryRavenConfiguration config)
        {
            config.Settings[Constants.Voron.AllowIncrementalBackups] = "true";
            config.Storage.Voron.AllowIncrementalBackups = true;
        }

        [Fact]
        public void Should_work()
        {
            using (var server = GetNewServer(runInMemory:false))
            using (var store = NewRemoteDocumentStore(requestedStorage:"voron",runInMemory:false, ravenDbServer:server))
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "1" }, "users/1");
                    session.SaveChanges();
                }

                store.DatabaseCommands
                     .GlobalAdmin
                     .StartBackup(BackupFolder + "_Will_Be_Deleted", null, false, store.DefaultDatabase)
                     .WaitForCompletion();

                store.DatabaseCommands
                    .GlobalAdmin
                    .StartBackup(BackupFolder + "_Will_Be_Deleted", null, true, store.DefaultDatabase)
                    .WaitForCompletion();

                using (var session = store.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    user.Name = "12";
                    session.Store(user);
                    session.SaveChanges();
                }

                //full backup
                store.DatabaseCommands
                    .GlobalAdmin
                    .StartBackup(BackupFolder, null, false, store.DefaultDatabase)
                    .WaitForCompletion();

                using (var session = store.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    user.Name = "123";
                    session.Store(user);
                    session.SaveChanges();
                }

                //incremental backup
                store.DatabaseCommands
                    .GlobalAdmin
                    .StartBackup(BackupFolder, null, true, store.DefaultDatabase)
                    .WaitForCompletion();

                using (var session = store.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    user.Name = "1234";
                    session.Store(user);
                    session.SaveChanges();
                }

                //incremental backup
                store.DatabaseCommands
                    .GlobalAdmin
                    .StartBackup(BackupFolder, null, true, store.DefaultDatabase)
                    .WaitForCompletion();

                //restore into "Restored" db
                store.DatabaseCommands
                    .GlobalAdmin
                    .StartRestore(new DatabaseRestoreRequest
                    {
                        BackupLocation = BackupFolder,
                        DatabaseName = "Restored",
                        GenerateNewDatabaseId = true
                    })
                    .WaitForCompletion();

                //make sure we have the latest changes that
                //were saved by both full and incremental backups
                using (var session = store.OpenSession("Restored"))
                {
                    var user = session.Load<User>("users/1");
                    Assert.NotNull(user);
                    Assert.Equal("1234", user.Name);
                }
            }

        }

        public override void Dispose()
        {
            IOExtensions.DeleteDirectory(BackupFolder);
            base.Dispose();
        }
    }
}
