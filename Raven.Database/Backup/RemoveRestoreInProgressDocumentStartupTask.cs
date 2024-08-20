//-----------------------------------------------------------------------
// <copyright file="RemoveBackupDocumentStartupTask.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Abstractions.Extensions;

namespace Raven35.Database.Backup
{
    /// <summary>
    /// Delete the restore in progress document, if it indicate a restore was in progress when the server crashed / shutdown
    /// we have to do that to enable the next restore to complete
    /// </summary>
    public class RemoveRestoreInProgressDocumentStartupTask : IStartupTask
    {
        public void Execute(DocumentDatabase database)
        {
            var oldBackup = database.Documents.Get(RestoreInProgress.RavenRestoreInProgressDocumentKey,null);
            if (oldBackup == null)
                return;

            database.Documents.Delete(RestoreInProgress.RavenRestoreInProgressDocumentKey, null, null);
        }
    }
}
