// -----------------------------------------------------------------------
//  <copyright file="RemoteDocumentReplicationConflictResolver.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Raven35.Abstractions.Data;
using Raven35.Bundles.Replication.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Database.Bundles.Replication.Plugins
{
    [PartNotDiscoverable]
    public class RemoteDocumentReplicationConflictResolver : AbstractDocumentReplicationConflictResolver
    {
        public static RemoteDocumentReplicationConflictResolver Instance = new RemoteDocumentReplicationConflictResolver();

        protected override bool TryResolve(string id, RavenJObject metadata, RavenJObject document, JsonDocument existingDoc,
                                        Func<string, JsonDocument> getDocument, out RavenJObject metadataToSave,
                                        out RavenJObject documentToSave)
        {
            metadataToSave = metadata;
            documentToSave = document;

            return true;
        }
    }
}
