// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2556.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Replication;
using Raven35.Client;
using Raven35.Client.Connection;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2556 : ReplicationBase
    {
        [Fact(Skip = "Flaky test")]
        public void FailoverBehaviorShouldBeReadFromServer()
        {
            IDocumentStore store1 = null;
            IDocumentStore store2 = null;

            try
            {
                store1 = CreateStore();
                store2 = CreateStore();

                Assert.Equal(FailoverBehavior.AllowReadsFromSecondaries, store1.Conventions.FailoverBehavior);

                RunReplication(store1, store2, clientConfiguration: new ReplicationClientConfiguration { FailoverBehavior = FailoverBehavior.ReadFromAllServers });

                var serverClient = ((ServerClient)store1.DatabaseCommands);
                GetReplicationInformer(serverClient).RefreshReplicationInformation(serverClient);

                Assert.Equal(FailoverBehavior.ReadFromAllServers, store1.Conventions.FailoverBehavior);
            }
            finally
            {
                var store1Snapshot = store1;
                if (store1Snapshot != null)
                {
                    var serverClient = ((ServerClient)store1Snapshot.DatabaseCommands);
                    GetReplicationInformer(serverClient).ClearReplicationInformationLocalCache(serverClient);

                    store1Snapshot.Dispose();
                }

                var store2Snapshot = store2;
                if (store2Snapshot != null)
                {
                    var serverClient = ((ServerClient)store2Snapshot.DatabaseCommands);
                    GetReplicationInformer(serverClient).ClearReplicationInformationLocalCache(serverClient);

                    store2Snapshot.Dispose();
                }
            }

        }
    }
}
