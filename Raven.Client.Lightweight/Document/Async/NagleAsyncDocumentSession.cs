using System;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Abstractions.Extensions;
using Raven35.Client.Connection.Async;

namespace Raven35.Client.Document.Async
{
    public class NagleAsyncDocumentSession : AsyncDocumentSession
    {
        private readonly DocumentStore documentStore;

        public NagleAsyncDocumentSession(string dbName, DocumentStore documentStore, IAsyncDatabaseCommands asyncDatabaseCommands, DocumentSessionListeners listeners, Guid id) : base(dbName, documentStore, asyncDatabaseCommands, listeners, id)
        {
            this.documentStore = documentStore;
        }

        public override async Task SaveChangesAsync(CancellationToken token = default(CancellationToken))
        {
            await asyncDocumentKeyGeneration.GenerateDocumentKeysForSaveChanges().WithCancellation(token).ConfigureAwait(false);

            using (EntityToJson.EntitiesToJsonCachingScope())
            {
                var data = PrepareForSaveChanges();
                if (data.Commands.Count == 0)
                    return;

                LogBatch(data);

                var task = documentStore.AddNagleData(DatabaseName, data);
                var result = await task.ConfigureAwait(false);
                UpdateBatchResults(result, data);
            }
        }
    }
}