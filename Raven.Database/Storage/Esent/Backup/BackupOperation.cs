//-----------------------------------------------------------------------
// <copyright file="BackupOperation.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Isam.Esent.Interop;
using Raven35.Abstractions;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.Logging;
using Raven35.Database.Backup;
using Raven35.Json.Linq;
using Raven35.Storage.Esent;
using Raven35.Storage.Esent.Backup;

namespace Raven35.Database.Storage.Esent.Backup
{
    public class BackupOperation : BaseBackupOperation
    {
        private readonly JET_INSTANCE instance;
        private string backupConfigPath;

        public BackupOperation(DocumentDatabase database, string backupSourceDirectory, string backupDestinationDirectory, bool incrementalBackup,
                               DatabaseDocument databaseDocument, ResourceBackupState state, CancellationToken cancellationToken)
            : base(database, backupSourceDirectory, backupDestinationDirectory, incrementalBackup, databaseDocument, state, cancellationToken)
        {
            instance = ((TransactionalStorage) database.TransactionalStorage).Instance;
            backupConfigPath = Path.Combine(backupDestinationDirectory, "RavenDB.Backup");
        }

        protected override bool BackupAlreadyExists
        {
            get { return Directory.Exists(backupDestinationDirectory) && File.Exists(backupConfigPath); }
        }

        protected override void ExecuteBackup(string backupPath, bool isIncrementalBackup, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(backupPath)) throw new ArgumentNullException("backupPath");

            // It doesn't seem to be possible to get the % complete from an esent backup, but any status msgs 
            // that is does give us are displayed live during the backup.
            var esentBackup = new EsentBackup(instance, backupPath, isIncrementalBackup ? BackupGrbit.Incremental : BackupGrbit.Atomic, token);
            esentBackup.Notify += UpdateBackupStatus;
            esentBackup.Execute();
        }

        protected override void OperationFinishedSuccessfully()
        {
            base.OperationFinishedSuccessfully();

            File.WriteAllText(backupConfigPath, "Backup completed " + SystemTime.UtcNow);
        }

        protected override bool CanPerformIncrementalBackup()
        {
            return BackupAlreadyExists;
        }
    }
}
