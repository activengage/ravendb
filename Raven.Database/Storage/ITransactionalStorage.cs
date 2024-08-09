//-----------------------------------------------------------------------
// <copyright file="ITransactionalStorage.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.MEF;
using Raven35.Database.Config;
using Raven35.Database.Impl;
using Raven35.Database.Impl.DTC;
using Raven35.Database.Indexing;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;
using Voron;

namespace Raven35.Database.Storage
{
    public interface ITransactionalStorage : IDisposable
    {
        /// <summary>
        /// This is used mostly for replication
        /// </summary>
        Guid Id { get; }

        IDocumentCacher DocumentCacher { get; }

        IDisposable DisableBatchNesting();

        IStorageActionsAccessor CreateAccessor();
        bool SkipConsistencyCheck { get;}
        void Batch(Action<IStorageActionsAccessor> action);
        void ExecuteImmediatelyOrRegisterForSynchronization(Action action);
        void Initialize(IUuidGenerator generator, OrderedPartCollection<AbstractDocumentCodec> documentCodecs, Action<string> putResourceMarker = null, Action<object, Exception> onError = null);
        Task StartBackupOperation(DocumentDatabase database, string backupDestinationDirectory, bool incrementalBackup, DatabaseDocument documentDatabase, ResourceBackupState state, CancellationToken cts);
        void Restore(DatabaseRestoreRequest restoreRequest, Action<string> output, InMemoryRavenConfiguration globalConfiguration);
        DatabaseSizeInformation GetDatabaseSize();
        long GetDatabaseCacheSizeInBytes();
        long GetDatabaseTransactionVersionSizeInBytes();
        StorageStats GetStorageStats();

        string FriendlyName { get; }
        bool HandleException(Exception exception);

        bool IsAlreadyInBatch { get; }
        bool SupportsDtc { get; }

        void Compact(InMemoryRavenConfiguration configuration, Action<string> output);
        Guid ChangeId();
        void ClearCaches();
        void DumpAllStorageTables();
        InFlightTransactionalState InitializeInFlightTransactionalState(DocumentDatabase self, Func<string, Etag, RavenJObject, RavenJObject, TransactionInformation, PutResult> put, Func<string, Etag, TransactionInformation, bool> delete);
        IList<string> ComputeDetailedStorageInformation(bool computeExactSizes, Action<string> progress, CancellationToken token);
        List<TransactionContextData> GetPreparedTransactions();

        object GetInFlightTransactionsInternalStateForDebugOnly();
        void DropAllIndexingInformation();
        ConcurrentDictionary<int, RemainingReductionPerLevel> GetScheduledReductionsPerViewAndLevel();
        /// <summary>
        /// Scheduled reduction tracking is a memory living entity it will get corrupted on a reset.
        /// The reset must occur while there are no map/reduce indexing activity going on.
        /// </summary>
        void ResetScheduledReductionsTracking();

        void RegisterTransactionalStorageNotificationHandler(ITransactionalStorageNotificationHandler handler);
    }

    public interface ITransactionalStorageNotificationHandler
    {
        void HandleTransactionalStorageNotification();
    }
}
