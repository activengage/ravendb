// -----------------------------------------------------------------------
//  <copyright file="RavenDB-3981.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Bundles.Replication;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3981 : ReplicationBase
    {
        [Fact]
        public void ConflictIndexAndTransformerShouldBeDeployedAutomaticallyWhenReplicationIsEnabled()
        {
            using (var store = CreateStore())
            {
                var index = new RavenConflictDocuments();
                var transformer = new RavenConflictDocumentsTransformer();

                Assert.NotNull(store.DatabaseCommands.GetIndex(index.IndexName));
                Assert.NotNull(store.DatabaseCommands.GetTransformer(transformer.TransformerName));
            }
        }
    }
}