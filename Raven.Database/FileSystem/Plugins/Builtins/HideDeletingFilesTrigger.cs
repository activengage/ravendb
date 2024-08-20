// -----------------------------------------------------------------------
//  <copyright file="HideVersionedFilesFromIndexingTrigger.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.Composition;
using Raven35.Abstractions.FileSystem;
using Raven35.Database.FileSystem.Util;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Database.FileSystem.Plugins.Builtins
{
    [InheritedExport(typeof(AbstractFileReadTrigger))]
    public class HideDeletingFilesTrigger : AbstractFileReadTrigger
    {
        public override ReadVetoResult AllowRead(string name, RavenJObject metadata, ReadOperation operation)
        {
            if (metadata.Value<bool>(SynchronizationConstants.RavenDeleteMarker))
                return ReadVetoResult.Ignore;

            if (name.EndsWith(RavenFileNameHelper.DeletingFileSuffix)) // such file should already have RavenDeleteMarker, but just in case
                return ReadVetoResult.Ignore;

            return ReadVetoResult.Allowed;
        }
    }
}
