using System.Collections.Generic;
using Raven35.Abstractions.Data;

namespace Raven35.Database.Indexing
{
    public interface IIndexingClassifier
    {
        Dictionary<Etag, List<IndexToWorkOn>> GroupMapIndexes(IList<IndexToWorkOn> indexes);
    }
}
