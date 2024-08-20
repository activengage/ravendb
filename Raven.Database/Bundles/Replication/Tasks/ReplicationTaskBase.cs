// -----------------------------------------------------------------------
//  <copyright file="ItemsReplicationTaskBase.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Bundles.Replication.Tasks;
using Raven35.Database.Storage;
using Raven35.Json.Linq;

namespace Raven35.Database.Bundles.Replication.Tasks
{
    public abstract class ReplicationTaskBase : IDisposable
    {
        protected readonly object EmptyRequestBody = new object();
        protected readonly DocumentDatabase Database;
        protected readonly HttpRavenRequestFactory HttpRavenRequestFactory;
        protected readonly ReplicationTask Replication;

        protected ReplicationTaskBase(DocumentDatabase database, HttpRavenRequestFactory httpRavenRequestFactory, ReplicationTask replication)
        {
            Database = database;
            HttpRavenRequestFactory = httpRavenRequestFactory;
            Replication = replication;
        }

        protected string GetTombstoneVersion(JsonDocument tombstone, string tombstoneKey, string key)
        {
            var version = tombstone.Metadata.Value<int?>(tombstoneKey) ?? 0;
            return key + "=" + version;
        }

        protected string GetDebugInformation()
        {
            return Constants.IsReplicatedUrlParamName + "=true&from=" + Uri.EscapeDataString(Database.ServerUrl);
        }

        protected List<JsonDocument> GetTombstones(string tombstoneListName, int start, int take, Func<ListItem, bool> wherePredicate = null)
        {
            List<JsonDocument> tombstones = null;

            Database.TransactionalStorage.Batch(actions =>
            {
                var getTombstones = actions
                    .Lists
                    .Read(tombstoneListName, start, take);

                if (wherePredicate != null)
                {
                    getTombstones = getTombstones.Where(wherePredicate);
                }

                tombstones = getTombstones.Select(x => new JsonDocument
                {
                    Etag = x.Etag,
                    Key = x.Key,
                    Metadata = x.Data,
                    DataAsJson = new RavenJObject()
                }).ToList();
            });

            return tombstones ?? new List<JsonDocument>();
        }

        protected List<ReplicationStrategy> GetReplicationDestinations(Predicate<ReplicationDestination> predicate = null)
        {
            var destinations = Replication.GetReplicationDestinations(predicate);
            return destinations.Where(x => x.IsETL == false).ToList();
        }

        public abstract void Dispose();
    }
}
