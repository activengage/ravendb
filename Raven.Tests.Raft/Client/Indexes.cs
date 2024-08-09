// -----------------------------------------------------------------------
//  <copyright file="Indexes.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Raven35.Abstractions.Cluster;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Abstractions.Util;
using Raven35.Bundles.Replication.Tasks;
using Raven35.Client.Connection;
using Raven35.Client.Indexes;
using Raven35.Tests.Common.Dto;

using Xunit.Extensions;

namespace Raven35.Tests.Raft.Client
{
    public class Indexes : RaftTestBase
    {
        private class Test_Index : AbstractIndexCreationTask<Person>
        {
            public Test_Index()
            {
                Map = persons => from person in persons select new { Name = person.Name };
            }
        }

        [Theory]
        [PropertyData("Nodes")]
        public void PutAndDeleteShouldBePropagated(int numberOfNodes)
        {
            var clusterStores = CreateRaftCluster(numberOfNodes, activeBundles: "Replication", configureStore: store => store.Conventions.FailoverBehavior = FailoverBehavior.ReadFromLeaderWriteToLeader);

            SetupClusterConfiguration(clusterStores);


            var store1 = clusterStores[0];
            servers.ForEach( server =>  
            {
                var sourceDatabase = AsyncHelpers.RunSync(()=> server.Server.GetDatabaseInternal(store1.DefaultDatabase)) ;
                var sourceReplicationTask = sourceDatabase.StartupTasks.OfType<ReplicationTask>().First();
                sourceReplicationTask.IndexReplication.TimeToWaitBeforeSendingDeletesOfIndexesToSiblings = TimeSpan.FromSeconds(0);
            });
            

            store1.DatabaseCommands.PutIndex("Test/Index", new Test_Index().CreateIndexDefinition(), true);

            var requestFactory = new HttpRavenRequestFactory();
            var replicationRequestUrl = string.Format("{0}/replication/replicate-indexes?op=replicate-all", store1.Url.ForDatabase(store1.DefaultDatabase));
            if (numberOfNodes > 1)
            {
                var replicationRequest = requestFactory.Create(replicationRequestUrl, HttpMethod.Post, new RavenConnectionStringOptions { Url = store1.Url });
                replicationRequest.ExecuteRequest();
            }

            using (ForceNonClusterRequests(clusterStores))
            {
                clusterStores.ForEach(store => WaitFor(store.DatabaseCommands, commands => commands.GetIndex("Test/Index") != null, TimeSpan.FromMinutes(1)));
            }

            store1.DatabaseCommands.DeleteIndex("Test/Index");
            if (numberOfNodes > 1)
            {
                var replicationRequest = requestFactory.Create(replicationRequestUrl, HttpMethod.Post, new RavenConnectionStringOptions { Url = store1.Url });
                replicationRequest.ExecuteRequest();
            }

            using (ForceNonClusterRequests(clusterStores))
            {
                clusterStores.ForEach(store => WaitFor(store.DatabaseCommands, commands => commands.GetIndex("Test/Index") == null, TimeSpan.FromMinutes(1)));
            }
        }
    }
}
