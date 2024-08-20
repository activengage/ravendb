// -----------------------------------------------------------------------
//  <copyright file="FailoverBehaviorServerSide.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading;
using Raven35.Abstractions.Replication;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection.Async;
using Raven35.Client.Document;
using Xunit;

namespace Raven35.Tests.Raft.Client
{
    public class FailoverBehaviorServerSide : RaftTestBase
    {
        [Fact]
        public void CanRedefineFailoverBehaviorFromServerSide_Cluster_to_replication()
        {
            CanRedefineFailoverBehaviorFromServerSide(FailoverBehavior.ReadFromLeaderWriteToLeader, FailoverBehavior.AllowReadsFromSecondaries);
        }

        [Fact]
        public void CanRedefineFailoverBehaviorFromServerSide_Replication_to_cluster()
        {
            CanRedefineFailoverBehaviorFromServerSide(FailoverBehavior.AllowReadsFromSecondaries, FailoverBehavior.ReadFromLeaderWriteToLeader);
        }

        private void CanRedefineFailoverBehaviorFromServerSide(FailoverBehavior storeBehavior, FailoverBehavior serverBehavior)
        {
            var clusterStores = CreateRaftCluster(1, "replication");

            using (var store = clusterStores[0])
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new ReplicationDocument
                    {
                        ClientConfiguration = new ReplicationClientConfiguration
                        {
                            FailoverBehavior = serverBehavior
                        }

                    }, "Raven/Replication/Destinations");
                    session.SaveChanges();
                }               

                using (var secondStore = new DocumentStore
                {
                    Url = store.Url,
                    Conventions =
                    {
                        FailoverBehavior = storeBehavior
                    },
                    DefaultDatabase = store.DefaultDatabase
                }.Initialize())
                {
                    var client = ((AsyncServerClient)secondStore.AsyncDatabaseCommands);
                    AsyncHelpers.RunSync(() => client.RequestExecuter.UpdateReplicationInformationIfNeededAsync(client, force: true));
                    Assert.True(SpinWait.SpinUntil(() => secondStore.Conventions.FailoverBehavior == serverBehavior, TimeSpan.FromSeconds(30)));
                }
            }
        }
    }
}