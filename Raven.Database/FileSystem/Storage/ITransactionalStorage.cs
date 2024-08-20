using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.FileSystem;
using Raven35.Abstractions.MEF;
using Raven35.Database.Config;
using Raven35.Database.FileSystem.Infrastructure;
using Raven35.Database.FileSystem.Plugins;

namespace Raven35.Database.FileSystem.Storage
{
    public interface ITransactionalStorage : IDisposable
    {
        Guid Id { get; }

        void Initialize(UuidGenerator generator, OrderedPartCollection<AbstractFileCodec> fileCodecs, Action<string> putResourceMarker = null);

        [DebuggerHidden, DebuggerNonUserCode, DebuggerStepThrough]
        void Batch(Action<IStorageActionsAccessor> action);

        string FriendlyName { get; }

        Task StartBackupOperation(DocumentDatabase systemDatabase, RavenFileSystem filesystem, string backupDestinationDirectory, bool incrementalBackup,
            FileSystemDocument fileSystemDocument, ResourceBackupState state, CancellationToken token);

        void Restore(FilesystemRestoreRequest restoreRequest, Action<string> output);

        void Compact(InMemoryRavenConfiguration configuration, Action<string> output);

        IDisposable DisableBatchNesting();

        Guid ChangeId();
    }
}
