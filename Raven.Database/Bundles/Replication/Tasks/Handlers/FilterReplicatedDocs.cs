// -----------------------------------------------------------------------
//  <copyright file="FilteringHandler.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Logging;
using Raven35.Bundles.Replication.Tasks;
using Raven35.Database.Actions;
using Raven35.Database.Prefetching;

namespace Raven35.Database.Bundles.Replication.Tasks.Handlers
{
    public class FilterReplicatedDocs : IReplicatedDocsHandler
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly DocumentActions docActions;
        private readonly ReplicationStrategy strategy;
        private readonly PrefetchingBehavior prefetchingBehavior;
        private readonly string destinationId;
        private readonly Etag lastEtag;

        public FilterReplicatedDocs(DocumentActions docActions, ReplicationStrategy strategy, PrefetchingBehavior prefetchingBehavior, string destinationId, Etag lastEtag)
        {
            this.docActions = docActions;
            this.strategy = strategy;
            this.prefetchingBehavior = prefetchingBehavior;
            this.destinationId = destinationId;
            this.lastEtag = lastEtag;
        }

        public IEnumerable<JsonDocument> Handle(IEnumerable<JsonDocument> docs)
        {
            return docs
                .Where(document =>
                {
                    string reason;
                    return strategy.FilterDocuments(destinationId, document.Key, document.Metadata, out reason) &&
                           prefetchingBehavior.FilterDocuments(document);
                });
        }
    }
}
