// -----------------------------------------------------------------------
//  <copyright file="LocalDocumentReplicationConflictResolver.cs" company="Hibernating Rhinos LTD">
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
    public class LatestDocumentReplicationConflictResolver : AbstractDocumentReplicationConflictResolver
    {
        public static LatestDocumentReplicationConflictResolver Instance = new LatestDocumentReplicationConflictResolver();

        protected override bool TryResolve(string id, RavenJObject metadata, RavenJObject document, JsonDocument existingDoc,
                                        Func<string, JsonDocument> getDocument, out RavenJObject metadataToSave,
                                        out RavenJObject documentToSave)
        {
            var remoteDocLastModified = metadata.Value<DateTime>(Constants.LastModified);
            var existingDocLastModified = existingDoc.LastModified??existingDoc.Metadata.Value<DateTime>(Constants.LastModified);

            if (remoteDocLastModified > existingDocLastModified)
            {
                metadataToSave = metadata;
                documentToSave = document;
            }
            else
            {
                metadataToSave = existingDoc.Metadata;
                documentToSave = existingDoc.DataAsJson;
            }
            
            return true;
        }
    }
}
