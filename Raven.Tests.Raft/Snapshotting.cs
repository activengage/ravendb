// -----------------------------------------------------------------------
//  <copyright file="ClusterBasic.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rachis.Transport;
using Raven35.Abstractions;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Client.Connection;
using Raven35.Database.Raft;
using Raven35.Database.Raft.Dto;
using Raven35.Database.Raft.Util;
using Raven35.Json.Linq;
using Raven35.Server;
using Xunit;

namespace Raven35.Tests.Raft
{
    public class SnaphshottingTest : RaftTestBase
    {
        [Fact]
        public void CanInstallSnapshot()
        {
            CreateRaftCluster(3, inMemory:false); // 3 nodes

            for (var i = 0; i < 5; i++)
            {
                var client = servers[0].Options.ClusterManager.Value.Client;
                client.SendClusterConfigurationAsync(new ClusterConfiguration {EnableReplication = false}).Wait(3000);
            }

            var leader = servers.FirstOrDefault(server => server.Options.ClusterManager.Value.IsLeader());
            Assert.NotNull(leader);

            var newServer = GetNewServer(GetPort(), runInMemory: false);

            var snapshotInstalledMre = new ManualResetEventSlim();

            newServer.Options.ClusterManager.Value.Engine.SnapshotInstalled += () => snapshotInstalledMre.Set();

            var allNodesFinishedJoining = new ManualResetEventSlim();
            leader.Options.ClusterManager.Value.Engine.TopologyChanged += command =>
            {
                if (command.Requested.AllNodeNames.All(command.Requested.IsVoter))
                {
                    allNodesFinishedJoining.Set();
                }
            };

            Assert.True(leader.Options.ClusterManager.Value.Engine.AddToClusterAsync(new NodeConnectionInfo
            {
                Name = RaftHelper.GetNodeName(newServer.SystemDatabase.TransactionalStorage.Id),
                Uri = RaftHelper.GetNodeUrl(newServer.SystemDatabase.Configuration.ServerUrl)
            }).Wait(20000));
            Assert.True(allNodesFinishedJoining.Wait(20000));

            Assert.True(snapshotInstalledMre.Wait(TimeSpan.FromSeconds(5)));
        }

        protected override void ModifyServer(RavenDbServer ravenDbServer)
        {
            ravenDbServer.Options.ClusterManager.Value.Engine.Options.MaxLogLengthBeforeCompaction = 4;
        }
    }
}
