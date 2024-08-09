// -----------------------------------------------------------------------
//  <copyright file="AsyncDocumentKeyGeneration.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading.Tasks;
using Raven35.Abstractions.Util;
using Raven35.Client.Extensions;
using Raven35.Json.Linq;

namespace Raven35.Client.Document.Async
{
    public class AsyncDocumentKeyGeneration
    {
        private readonly LinkedList<object> entitiesStoredWithoutIDs = new LinkedList<object>();

        public delegate bool TryGetValue(object key, out InMemoryDocumentSessionOperations.DocumentMetadata metadata);

        public delegate string ModifyObjectId(string id, object entity, RavenJObject metadata);

        private readonly InMemoryDocumentSessionOperations session;
        private readonly TryGetValue tryGetValue;
        private readonly ModifyObjectId modifyObjectId;

        public AsyncDocumentKeyGeneration(InMemoryDocumentSessionOperations session,TryGetValue tryGetValue, ModifyObjectId modifyObjectId)
        {
            this.session = session;
            this.tryGetValue = tryGetValue;
            this.modifyObjectId = modifyObjectId;
        }

        public Task GenerateDocumentKeysForSaveChanges()
        {
            if (entitiesStoredWithoutIDs.Count != 0)
            {
                var entity = entitiesStoredWithoutIDs.First.Value;
                entitiesStoredWithoutIDs.RemoveFirst();

                InMemoryDocumentSessionOperations.DocumentMetadata metadata;
                if (tryGetValue(entity, out metadata))
                {
                    return session.GenerateDocumentKeyForStorageAsync(entity)
                        .ContinueWith(task => metadata.Key = modifyObjectId(task.Result, entity, metadata.Metadata))
                        .ContinueWithTask(GenerateDocumentKeysForSaveChanges);
                }
            }

            return new CompletedTask();
        }

        public void Add(object entity)
        {
            entitiesStoredWithoutIDs.AddLast(entity);
        }
    }
}
