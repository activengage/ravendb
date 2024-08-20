using Lucene.Net.Index;
using Raven35.Abstractions.MEF;
using Raven35.Database.Plugins;

namespace Raven35.Database.Indexing
{
    public class IndexReaderWarmersWrapper: IndexWriter.IndexReaderWarmer 
    {
        public string IndexName { get; private set; }
        public IndexReaderWarmersWrapper(string indexName, OrderedPartCollection<AbstractIndexReaderWarmer> indexReaderWarmers)
        {
            IndexName = indexName;
            _indexReaderWarmers = indexReaderWarmers;
        }

        private readonly OrderedPartCollection<AbstractIndexReaderWarmer> _indexReaderWarmers;

        public override void Warm(IndexReader reader)
        {
            if (_indexReaderWarmers == null) return;
            foreach (var warmer in _indexReaderWarmers)
            {
                warmer.Value.WarmIndexReader(IndexName, reader);
            }
        }
    }
}
