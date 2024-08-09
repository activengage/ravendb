using Raven35.Abstractions.Util.Streams;

namespace Raven35.Database.Storage.Voron.Impl
{
    using System;
    using System.Collections.Concurrent;

    internal class Table : TableBase
    {
        private readonly ConcurrentDictionary<string, Index> tableIndexes;

        public Table(string tableName, IBufferPool bufferPool, params string[] indexNames)
            : base(tableName, bufferPool)
        {
            tableIndexes = new ConcurrentDictionary<string, Index>();

            foreach (var indexName in indexNames)
                GetIndex(indexName);
        }

        public Index GetIndex(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ArgumentNullException(indexName);

            var indexKey = GetIndexKey(indexName);
            return tableIndexes.GetOrAdd(indexKey, indexTreeName => new Index(indexTreeName, BufferPool));
        }

        public string GetIndexKey(string indexName)
        {
            return TableName + "_" + indexName;
        }
    }
}
