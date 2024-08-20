using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;

namespace Raven35.Database.Indexing
{
    public interface IIndexExtension : IDisposable
    {
        void OnDocumentsIndexed(IEnumerable<Document> documents, Analyzer searchAnalyzer);
        string Name { get; }
    }
}
