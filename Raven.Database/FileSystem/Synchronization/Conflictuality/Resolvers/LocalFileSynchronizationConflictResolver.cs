// -----------------------------------------------------------------------
//  <copyright file="LocalFileSynchronizationConflictResolver.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.Composition;
using Raven35.Abstractions.FileSystem;
using Raven35.Json.Linq;

namespace Raven35.Database.FileSystem.Synchronization.Conflictuality.Resolvers
{
    [PartNotDiscoverable]
    public class LocalFileSynchronizationConflictResolver : AbstractFileSynchronizationConflictResolver
    {
        public static LocalFileSynchronizationConflictResolver Instance = new LocalFileSynchronizationConflictResolver();

        public override bool TryResolve(string fileName, RavenJObject localMedatada, RavenJObject remoteMetadata, out ConflictResolutionStrategy resolutionStrategy)
        {
            resolutionStrategy = ConflictResolutionStrategy.CurrentVersion;
            return true;
        }
    }
}
