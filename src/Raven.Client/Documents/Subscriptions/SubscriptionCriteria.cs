// -----------------------------------------------------------------------
//  <copyright file="SubscriptionCriteria.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Raven.Client.Documents.Replication;
using Raven.Client.Documents.Replication.Messages;
using Sparrow.Json;
using Sparrow.Json.Parsing;

namespace Raven.Client.Documents.Subscriptions
{
    public class SubscriptionRaftState:IDatabaseTask
    {
        public SubscriptionRaftState()
        {

        }
        public SubscriptionCriteria Criteria { get; set; }
        public ChangeVectorEntry[] ChangeVector { get; set; }
        public long SubscriptionId { get; set; }
        
        public ulong GetTaskKey()
        {
            return (ulong)SubscriptionId;
        }

        public DynamicJsonValue ToJson()
        {
            return new DynamicJsonValue
            {
                [nameof(this.Criteria)] = new DynamicJsonValue
                {
                    [nameof(SubscriptionCriteria.Collection)] = Criteria.Collection,
                    [nameof(SubscriptionCriteria.FilterJavaScript)] = Criteria.FilterJavaScript
                },
                [nameof(ChangeVector)] = ChangeVector?.ToJson(),
                [nameof(SubscriptionId)] = SubscriptionId

            };
        }
    }

    public class SubscriptionCreationParams
    {
        public SubscriptionCriteria Criteria { get; set; }
        public ChangeVectorEntry[] ChangeVector { get; set; }
    }

    public class SubscriptionCreationParams<T>
    {
        public SubscriptionCreationParams()
        {
            
        }
        public SubscriptionCriteria<T> Criteria { get; set; }
        public ChangeVectorEntry[] ChangeVector { get; set; }
    }
    public class SubscriptionCriteria : IFillFromBlittableJson
    {
        public SubscriptionCriteria()
        {
            // for deserialization
        }

        public SubscriptionCriteria(string collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Collection = collection;
        }

        public string Collection { get; private set; }
        public string FilterJavaScript { get; set; }

        public void FillFromBlittableJson(BlittableJsonReaderObject json)
        {
            if (json == null)
                return;

            string collection;
            if (json.TryGet(nameof(Collection), out collection))
                Collection = collection;

            string filterJavaScript;
            if (json.TryGet(nameof(FilterJavaScript), out filterJavaScript))
                FilterJavaScript = filterJavaScript;
        }
    }

    public class SubscriptionCriteria<T>
    {
        public SubscriptionCriteria()
        {

        }
        public string FilterJavaScript { get; set; }
    }
}