// -----------------------------------------------------------------------
//  <copyright file="HideSyncingFileTrigger.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.Composition;

using Raven35.Database.FileSystem.Util;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Database.FileSystem.Plugins.Builtins
{
    [InheritedExport(typeof(AbstractFileReadTrigger))]
    public class HideDownloadingFileTrigger : AbstractFileReadTrigger
    {
        public override ReadVetoResult AllowRead(string name, RavenJObject metadata, ReadOperation operation)
        {
            if (name.EndsWith(RavenFileNameHelper.DownloadingFileSuffix))
                return ReadVetoResult.Ignore;

            return ReadVetoResult.Allowed;
        }
    }
}