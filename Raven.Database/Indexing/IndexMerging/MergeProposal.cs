using System.Collections.Generic;

namespace Raven35.Database.Indexing.IndexMerging
{
    internal class MergeProposal
    {

        public List<IndexData> ProposedForMerge = new List<IndexData>();
        public IndexData MergedData { get; set; }

        public string IndexMergeSuggestion { get; set; }


    }
}
