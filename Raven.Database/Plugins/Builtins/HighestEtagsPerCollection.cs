// -----------------------------------------------------------------------
//  <copyright file="IndexStalenessDetectionOptimizer.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Database.Plugins.Builtins
{
    public class HighestEtagsPerCollection : AbstractPutTrigger
    {
        public override void AfterCommit(string key, RavenJObject document, RavenJObject metadata, Etag etag)
        {
            var entityName = metadata.Value<string>(Constants.RavenEntityName);

            if (entityName == null)
                return;

            Database.LastCollectionEtags.Update(entityName, etag);
        }
    }
}
