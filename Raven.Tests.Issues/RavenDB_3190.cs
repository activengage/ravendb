// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3190.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Threading;

using Raven35.Abstractions.Data;
using Raven35.Tests.Bundles.PeriodicExports;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3190 : RavenTest
    {
        [Fact]
        public void DisablingBackupShouldCauseLocalFolderBackupsToStop()
        {
            var backupPath = NewDataPath("BackupFolder", forceCreateDir: true);
            using (var store = NewDocumentStore(activeBundles: "PeriodicExport"))
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new PeriodicBackupTests.User { Name = "oren" });
                    var periodicExportSetup = new PeriodicExportSetup
                    {
                        LocalFolderName = backupPath,
                        IntervalMilliseconds = 25,
                        Disabled = true
                    };

                    session.Store(periodicExportSetup, PeriodicExportSetup.RavenDocumentKey);

                    session.SaveChanges();
                }

                Thread.Sleep(5000);

                Assert.Null(store.DatabaseCommands.Get(PeriodicExportStatus.RavenDocumentKey));

                Assert.Equal(0, Directory.GetFiles(backupPath).Length);
                Assert.Equal(0, Directory.GetDirectories(backupPath).Length);
            }
        }
    }
}
