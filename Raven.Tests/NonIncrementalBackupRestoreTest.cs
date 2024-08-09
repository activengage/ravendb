using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Client.Indexes;
using Raven35.Database;
using Raven35.Database.Actions;
using Raven35.Database.Config;
using Raven35.Database.Extensions;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests
{
    public class NonIncrementalBackupRestoreTest : TransactionalStorageTestBase
    {
        private readonly string DataDir;
        private readonly string BackupDir;

        private DocumentDatabase db;

        public NonIncrementalBackupRestoreTest()
        {
            BackupDir = NewDataPath("BackupDatabase");
            DataDir = NewDataPath("DataDirectory");
        }

        public override void Dispose()
        {
            db.Dispose();
            base.Dispose();
        }


        private void InitializeDocumentDatabase(string storageName)
        {
            db = new DocumentDatabase(new RavenConfiguration
            {
                DefaultStorageTypeName = storageName,
                DataDirectory = DataDir,
                RunInMemory = false,
                RunInUnreliableYetFastModeThatIsNotSuitableForProduction = false,
                Settings =
                {
                    {"Raven/Esent/CircularLog", "false"}
                }
            }, null);
            db.Indexes.PutIndex(new RavenDocumentsByEntityName().IndexName, new RavenDocumentsByEntityName().CreateIndexDefinition());
        }

        [Theory]
        [PropertyData("Storages")]
        public void NonIncrementalBackup_Restore_CanReadDocument(string storageName)
        {
            InitializeDocumentDatabase(storageName);
            IOExtensions.DeleteDirectory(BackupDir);

            db.Documents.Put("Foo", null, RavenJObject.Parse("{'email':'foo@bar.com'}"), new RavenJObject(), null);

            db.Maintenance.StartBackup(BackupDir, false, new DatabaseDocument(), new ResourceBackupState());
            WaitForBackup(db, true);

            db.Dispose();
            IOExtensions.DeleteDirectory(DataDir);

            MaintenanceActions.Restore(new RavenConfiguration
            {
                DefaultStorageTypeName = storageName,
                DataDirectory = DataDir,
                RunInMemory = false,
                RunInUnreliableYetFastModeThatIsNotSuitableForProduction = false,
                Settings =
                {
                    {"Raven/Esent/CircularLog", "false"},
                    {"Raven35.Voron/AllowIncrementalBackups", "true"}
                }

            }, new DatabaseRestoreRequest
            {
                BackupLocation = BackupDir,
                DatabaseLocation = DataDir,
                Defrag = true
            }, s => { });

            db = new DocumentDatabase(new RavenConfiguration { DataDirectory = DataDir }, null);

            var fetchedData = db.Documents.Get("Foo", null);
            Assert.NotNull(fetchedData);

            var jObject = fetchedData.ToJson();
            Assert.NotNull(jObject);
            Assert.Equal("foo@bar.com", jObject.Value<string>("email"));

            db.Dispose();
        }

        [Theory]
        [PropertyData("Storages")]
        public void NonIncrementalBackup_Restore_DataDirectoryAlreadyExists_ExceptionThrown(string storageName)
        {
            InitializeDocumentDatabase(storageName);
            IOExtensions.DeleteDirectory(BackupDir);

            db.Documents.Put("Foo", null, RavenJObject.Parse("{'email':'foo@bar.com'}"), new RavenJObject(), null);

            db.Maintenance.StartBackup(BackupDir, false, new DatabaseDocument(), new ResourceBackupState());
            WaitForBackup(db, true);

            db.Dispose();

            //data directiory still exists --> should fail to restore backup
            Assert.Throws<IOException>(() => 
                MaintenanceActions.Restore(new RavenConfiguration
                {
                    DefaultStorageTypeName = storageName,
                    DataDirectory = DataDir,
                    RunInMemory = false,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = false,
                    Settings =
                    {
                        {"Raven/Esent/CircularLog", "false"}
                    }

                }, new DatabaseRestoreRequest
                {
                    BackupLocation = BackupDir,
                    DatabaseLocation = DataDir,
                    Defrag = true
                }, s => { }));
        }

        [Theory]
        [PropertyData("Storages")]
        public void NonIncrementalBackup_Restore_CanReadDocumentWithMissingIndexDir(string storageName)
        {
            InitializeDocumentDatabase(storageName);
            IOExtensions.DeleteDirectory(BackupDir);

            db.Documents.Put("Foo", null, RavenJObject.Parse("{'email':'foo@bar.com'}"), new RavenJObject(), null);

            db.Maintenance.StartBackup(BackupDir, false, new DatabaseDocument(), new ResourceBackupState());
            WaitForBackup(db, true);

            db.Dispose();
            IOExtensions.DeleteDirectory(DataDir);

            IOExtensions.DeleteDirectory(Path.Combine(BackupDir, "IndexDefinitions")); // deliberate delete directory
            IOExtensions.DeleteDirectory(Path.Combine(BackupDir, "Indexes")); // deliberate delete directory

            //index directiory doesn't exists --> should NOT fail to restore backup
            Assert.DoesNotThrow(() =>
                MaintenanceActions.Restore(new RavenConfiguration
                {
                    DefaultStorageTypeName = storageName,
                    DataDirectory = DataDir,
                    RunInMemory = false,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = false,
                    Settings =
                    {
                        {"Raven/Esent/CircularLog", "false"}
                    }

                }, new DatabaseRestoreRequest
                {
                    BackupLocation = BackupDir,
                    DatabaseLocation = DataDir,
                    Defrag = true
                }, s => { }));

            db = new DocumentDatabase(new RavenConfiguration { DataDirectory = DataDir }, null);

            var fetchedData = db.Documents.Get("Foo", null);
            Assert.NotNull(fetchedData);

            var jObject = fetchedData.ToJson();
            Assert.NotNull(jObject);
            Assert.Equal("foo@bar.com", jObject.Value<string>("email"));

            db.Dispose();
        }

        [Theory]
        [PropertyData("Storages")]
        public void NonIncrementalBackup_Restore_CanReadDocumentWithCorruptedIndex(string storageName)
        {
            InitializeDocumentDatabase(storageName);
            IOExtensions.DeleteDirectory(BackupDir);

            db.Documents.Put("Foo", null, RavenJObject.Parse("{'email':'foo@bar.com'}"), new RavenJObject(), null);

            db.Maintenance.StartBackup(BackupDir, false, new DatabaseDocument(), new ResourceBackupState());
            WaitForBackup(db, true);

            db.Dispose();
            IOExtensions.DeleteDirectory(DataDir);

            var indexFile = Path.Combine(BackupDir, "IndexDefinitions", "1.index");
            string text = File.ReadAllText(indexFile);
            text = text.Replace("from", "corrupt");
            File.WriteAllText(indexFile, text); // deliberately corrupt index

            //index is corrupted --> should NOT fail to restore backup
            Assert.DoesNotThrow(() =>
                MaintenanceActions.Restore(new RavenConfiguration
                {
                    DefaultStorageTypeName = storageName,
                    DataDirectory = DataDir,
                    RunInMemory = false,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = false,
                    Settings =
                    {
                        {"Raven/Esent/CircularLog", "false"}
                    }

                }, new DatabaseRestoreRequest
                {
                    BackupLocation = BackupDir,
                    DatabaseLocation = DataDir,
                    Defrag = true
                }, s => { }));

            db = new DocumentDatabase(new RavenConfiguration { DataDirectory = DataDir }, null);

            var fetchedData = db.Documents.Get("Foo", null);
            Assert.NotNull(fetchedData);

            var jObject = fetchedData.ToJson();
            Assert.NotNull(jObject);
            Assert.Equal("foo@bar.com", jObject.Value<string>("email"));

            db.Dispose();
        }


    }
}
