// -----------------------------------------------------------------------
//  <copyright file="ClusterDatabases.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Rachis.Transport;

using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Connection;
using Raven35.Client.Connection.Async;
using Raven35.Client.Document;
using Raven35.Database.Raft.Util;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests.Raft
{
    public class Building
    {
        public int Floors { get; set; }
    }


    public class RavenDB_4845 : RaftTestBase
    {
        [Fact]
        public void DatabaseShouldBeCreatedOnAllNodes()
        {
           
            var clusterStores = CreateRaftCluster(5, activeBundles: "Replication", configureStore: store => store.Conventions.FailoverBehavior = FailoverBehavior.ReadFromAllWriteToLeaderWithFailovers);
            SetupClusterConfiguration(clusterStores);
            var store0 = clusterStores[0];

            using (ForceNonClusterRequests(clusterStores))
            {
                for (int k = 0; k < 5; k++)
                {
                    for (var i = 0; i < clusterStores.Count; i++)
                    {
                        using (var session = clusterStores[i].OpenSession())
                        {
                            session.Store(new ReplicationConfig
                            {
                                DocumentConflictResolution = StraightforwardConflictResolution.ResolveToLatest
                            }, Constants.RavenReplicationConfig);
                            session.SaveChanges();
                        }
                    }
                }
            }


            using (ForceNonClusterRequests(clusterStores))
            {
                    
                for (int k = 0; k < 5; k++)
                {
                    for (var i = 0; i < clusterStores.Count; i++)
                    {
                
                        clusterStores[i].DatabaseCommands.Put($"Buildings/{k}", null, RavenJObject.FromObject(new Building() {Floors = 1}), new RavenJObject());
                        for (var j = 0; j < (i - 1)*4; j++)
                        {
                            clusterStores[i].DatabaseCommands.Put($"Buildings/{k}", null, RavenJObject.FromObject(new Building {Floors = 1}), new RavenJObject());
                        }
                    }
                }
            }

            clusterStores.ForEach(store => ((ServerClient)store.DatabaseCommands).RequestExecuter.UpdateReplicationInformationIfNeededAsync((AsyncServerClient)store.AsyncDatabaseCommands, force: true).Wait());

            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    using (var session = store0.OpenSession(new OpenSessionOptions()
                    {
                        ForceReadFromMaster = true,
                    }))
                    {
                        session.Advanced.UseOptimisticConcurrency = true;
                        var bucket = session.Load<Building>($"Buildings/{j}");
                        bucket.Floors++;
                        session.SaveChanges();
                    }
                }
            }
                
            for (int j = 1; j < 5; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    using (var session = store0.OpenSession(new OpenSessionOptions()
                    {
                        ForceReadFromMaster = true
                    }))
                    {
                        var bucket = session.Load<Building>($"Buildings/{j}");
                        bucket = session.Load<Building>($"Buildings/{j - 1}");
                        bucket.Floors++;
                        session.SaveChanges();
                    }
                }
            }

            foreach (var store in clusterStores)
            {
                store.Dispose();
            }
            
           

        }
    }
}
