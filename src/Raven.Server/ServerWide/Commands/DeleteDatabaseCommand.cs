﻿using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Server.Rachis;
using Raven.Server.ServerWide.Context;
using Raven.Server.Utils;
using Sparrow.Json.Parsing;

namespace Raven.Server.ServerWide.Commands
{
    public class DeleteDatabaseCommand : UpdateDatabaseCommand
    {
        public string[] ClusterNodes;
        public bool HardDelete;
        public string[] FromNodes;
        public bool UpdateReplicationFactor = true;
        public int? ShardNumber = null;

        public DeleteDatabaseCommand()
        {
        }

        public DeleteDatabaseCommand(string databaseName, string uniqueRequestId) : base(databaseName, uniqueRequestId)
        {
            ErrorOnDatabaseDoesNotExists = false;

            if (ShardHelper.TryGetShardNumberAndDatabaseName(DatabaseName, out DatabaseName, out var shard))
                ShardNumber = shard;
        }

        public override void Initialize(ServerStore serverStore, ClusterOperationContext context)
        {
            ClusterNodes = serverStore.GetClusterTopology(context).AllNodes.Keys.ToArray();
        }

        public override void UpdateDatabaseRecord(DatabaseRecord record, long etag)
        {
            var deletionInProgressStatus = HardDelete ? DeletionInProgressStatus.HardDelete
                : DeletionInProgressStatus.SoftDelete;

            if (record.DeletionInProgress == null)
            {
                record.DeletionInProgress = new Dictionary<string, DeletionInProgressStatus>();
            }

            if (FromNodes != null && FromNodes.Length > 0) 
            {
                foreach (var node in FromNodes)
                {
                    if (record.IsSharded == false)
                    {
                        RemoveDatabaseFromSingleNode(record, record.Topology, node, shardNumber: null, deletionInProgressStatus);
                    }
                    else
                    {
                        if (ShardNumber.HasValue)
                        {
                            if (record.Sharding.Shards.ContainsKey(ShardNumber.Value) == false)
                                throw new RachisApplyException($"The requested shard '{ShardNumber}' doesn't exists in '{record.DatabaseName}'");

                            RemoveDatabaseFromSingleNode(record, record.Sharding.Shards[ShardNumber.Value], node, shardNumber: ShardNumber, deletionInProgressStatus);
                            return;
                        }

                        throw new InvalidOperationException($"Deleting entire sharded database {DatabaseName} from a specific node is not allowed.");
                    }
                }
            }
            else
            {
                if (record.IsSharded == false)
                {
                    RemoveDatabaseFromAllNodes(record, record.Topology, shardNumber: null, deletionInProgressStatus);
                }
                else
                {
                    if (ShardNumber.HasValue)
                        throw new InvalidOperationException($"Deleting an entire shard group (shard {ShardNumber.Value}) from the database is not allowed.");

                    foreach (var (shardNumber, topology) in record.Sharding.Shards)
                    {
                        record.Sharding.Shards[shardNumber] = RemoveDatabaseFromAllNodes(record, record.Sharding.Shards[shardNumber], shardNumber, deletionInProgressStatus);
                    }
                }
            }
        }
        
        private DatabaseTopology RemoveDatabaseFromAllNodes(DatabaseRecord record, DatabaseTopology topology, int? shardNumber, DeletionInProgressStatus deletionInProgressStatus)
        {
            var allNodes = topology.AllNodes.Distinct();

            foreach (var node in allNodes)
            {
                if (ClusterNodes.Contains(node))
                    record.DeletionInProgress[DatabaseRecord.GetKeyForDeletionInProgress(node, shardNumber)] = deletionInProgressStatus;
            }

            return new DatabaseTopology {Stamp = record.Topology?.Stamp, ReplicationFactor = 0};
        }

        private void RemoveDatabaseFromSingleNode(DatabaseRecord record, DatabaseTopology topology, string node, int? shardNumber, DeletionInProgressStatus deletionInProgressStatus)
        {
            if (topology.RelevantFor(node) == false)
            {
                DatabaseDoesNotExistException.ThrowWithMessage(ShardNumber.HasValue ? ShardHelper.ToShardName(record.DatabaseName, ShardNumber.Value) : record.DatabaseName,
                    $"Request to delete database from node '{node}' failed because it does not exist on this node.");
            }

            // rehabs will be removed only once the replication sent all the documents to the mentor
            if (topology.Rehabs.Contains(node) == false)
                topology.RemoveFromTopology(node);

            if (UpdateReplicationFactor)
            {
                topology.ReplicationFactor--;
            }

            if (ClusterNodes.Contains(node))
                record.DeletionInProgress[DatabaseRecord.GetKeyForDeletionInProgress(node, shardNumber)] = deletionInProgressStatus;
        }
        public override void FillJson(DynamicJsonValue json)
        {
            json[nameof(HardDelete)] = HardDelete;
            json[nameof(RaftCommandIndex)] = RaftCommandIndex;
            json[nameof(UpdateReplicationFactor)] = UpdateReplicationFactor;
            json[nameof(ShardNumber)] = ShardNumber;
            if (FromNodes != null)
            {
              json[nameof(FromNodes)] = new DynamicJsonArray(FromNodes);
            }
        }
    }
}
