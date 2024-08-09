using System;

namespace Raven35.Abstractions.Data
{
    public enum UuidType : byte
    {
        Documents = 1,

        [Obsolete("Use RavenFS instead.")]
        Attachments = 2,
        DocumentTransactions = 3,
        MappedResults = 4,
        ReduceResults = 5,
        ScheduledReductions = 6,
        Queue = 7,
        Tasks = 8,
        Indexing = 9,
        DocumentReferences = 11,
        Subscriptions = 12,
        Transformers = 13,
        Cluster = 14,
        Licensing = 15,
        SupportCoverage = 16
    }
}
