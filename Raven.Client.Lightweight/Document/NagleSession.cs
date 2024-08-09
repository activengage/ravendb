using System;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection;

namespace Raven35.Client.Document
{
    public class NagleSession : DocumentSession
    {
        private readonly DocumentStore documentStore;

        public NagleSession(string dbName, DocumentStore documentStore, DocumentSessionListeners listeners, Guid id, IDatabaseCommands databaseCommands)
            : base(dbName, documentStore, listeners, id, databaseCommands)
        {
            this.documentStore = documentStore;
        }

        /// <summary>
        /// Saves all the changes to the Raven35.Server.
        /// </summary>
        public override void SaveChanges()
        {
            using (EntityToJson.EntitiesToJsonCachingScope())
            {
                var data = PrepareForSaveChanges();
                if (data.Commands.Count == 0)
                    return;

                IncrementRequestCount();
                LogBatch(data);

                var task = documentStore.AddNagleData(DatabaseName, data);
                var batchResults = AsyncHelpers.RunSync(() => task);
                if (batchResults == null)
                    throw new InvalidOperationException("Cannot call Save Changes after the document store was disposed.");

                UpdateBatchResults(batchResults, data);
            }
        }
    }
}