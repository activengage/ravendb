// -----------------------------------------------------------------------
//  <copyright file="RemoteAttachmentReplicationConflictResolver.cs" company="Hibernating Rhinos LTD">
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
    [Obsolete("Use RavenFS instead.")]
    public class RemoteAttachmentReplicationConflictResolver : AbstractAttachmentReplicationConflictResolver
    {
        public static RemoteAttachmentReplicationConflictResolver Instance = new RemoteAttachmentReplicationConflictResolver();

        protected override bool TryResolve(string id, RavenJObject metadata, byte[] data, Attachment existingAttachment, Func<string, Attachment> getAttachment,
                                        out RavenJObject metadataToSave, out byte[] dataToSave)
        {
            metadataToSave = metadata;
            dataToSave = data;

            return true;
        }
    }
}
