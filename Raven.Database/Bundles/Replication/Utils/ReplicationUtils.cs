// -----------------------------------------------------------------------
//  <copyright file="ReplicationUtils.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Bundles.Replication.Tasks;

namespace Raven35.Database.Bundles.Replication.Utils
{
    public static class ReplicationUtils
    {
        internal static ReplicationStatistics GetReplicationInformation(DocumentDatabase database)
        {
            var mostRecentDocumentEtag = Etag.Empty;
            var mostRecentAttachmentEtag = Etag.Empty;
            database.TransactionalStorage.Batch(accessor =>
            {
                mostRecentDocumentEtag = accessor.Staleness.GetMostRecentDocumentEtag();
                mostRecentAttachmentEtag = accessor.Staleness.GetMostRecentAttachmentEtag();
            });

            var replicationTask = database.StartupTasks.OfType<ReplicationTask>().FirstOrDefault();
            var replicationStatistics = new ReplicationStatistics
            {
                Self = database.ServerUrl,
                MostRecentDocumentEtag = mostRecentDocumentEtag,
                MostRecentAttachmentEtag = mostRecentAttachmentEtag,
                Stats = replicationTask == null ? new List<DestinationStats>() : replicationTask.DestinationStats.Values.ToList()
            };

            return replicationStatistics;
        }
    }
}
