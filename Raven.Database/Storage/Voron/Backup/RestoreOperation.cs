using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Logging;
using Raven35.Database.Config;
using System;
using System.IO;
using Raven35.Database.Extensions;
using Voron;
using Voron.Impl.Backup;

namespace Raven35.Database.Storage.Voron.Backup
{
    internal class RestoreOperation : BaseRestoreOperation
    {
        public RestoreOperation(DatabaseRestoreRequest restoreRequest, InMemoryRavenConfiguration configuration, InMemoryRavenConfiguration globalConfiguration, Action<string> operationOutputCallback)
            : base(restoreRequest, configuration, globalConfiguration, operationOutputCallback)
        {
            journalLocation = GenerateJournalLocation(_restoreRequest, configuration).ToFullPath();
        }

        private string GenerateJournalLocation(DatabaseRestoreRequest databaseRestoreRequest, InMemoryRavenConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(databaseRestoreRequest.JournalsLocation))
                return databaseRestoreRequest.JournalsLocation;

            if (configuration != null)
            {
                var customJournalPath = configuration.Settings[Constants.RavenTxJournalPath];
                if (!string.IsNullOrWhiteSpace(customJournalPath))
                    return customJournalPath;

                return configuration.DataDirectory;
            }

            return databaseRestoreRequest.DatabaseLocation;
        }

        protected override bool IsValidBackup(string backupFilename)
        {
            return File.Exists(BackupFilenamePath(backupFilename));
        }


        public override void Execute()
        {
            ValidateRestorePreconditionsAndReturnLogsPath(BackupMethods.Filename);

            try
            {
                CopyIndexes();
                CopyIndexDefinitions();

                //if we have full & incremental in the same folder, do full restore first
                var fullBackupFilename = Path.Combine(backupLocation, "RavenDB.Voron.Backup");
                if (File.Exists(fullBackupFilename))
                {
                    BackupMethods.Full.Restore(fullBackupFilename, Configuration.DataDirectory, journalLocation);
                }

                using (var options = StorageEnvironmentOptions.ForPath(Configuration.DataDirectory, journalPath: journalLocation))
                {
                    var backupPaths = Directory.GetDirectories(backupLocation, "Inc*")
                        .OrderBy(dir => dir)
                        .Select(dir => Path.Combine(dir, BackupMethods.Filename))
                        .ToList();
                    BackupMethods.Incremental.Restore(options, backupPaths);
                }
            }
            catch (Exception e)
            {
                output("Restore Operation: Failure! Could not restore database!");
                output(e.ToString());
                log.WarnException("Could not complete restore", e);
                throw;
            }
        }

    }
}
