using System.Collections.Generic;

namespace Raven35.Abstractions.Indexing
{
    public class IndexMergeResults
    {
        public Dictionary<string, string> Unmergables = new Dictionary<string, string>(); // index name, reason

        public List<MergeSuggestions> Suggestions = new List<MergeSuggestions>();
    }
}
