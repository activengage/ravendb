#if !DNXCORE50
using Lucene.Net.Analysis;
#endif
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class Posts_ByTitleAndContent : AbstractIndexCreationTask<Post>
    {
        public Posts_ByTitleAndContent()
        {
            Map = posts => from post in posts
                           select new
                           {
                               post.Title,
                               post.Desc
                           };

            Stores.Add(x => x.Title, FieldStorage.Yes);
            Stores.Add(x => x.Desc, FieldStorage.Yes);

#if !DNXCORE50
            Analyzers.Add(x => x.Title, typeof(SimpleAnalyzer).FullName);
            Analyzers.Add(x => x.Desc, typeof(SimpleAnalyzer).FullName);
#else
            Analyzers.Add(x => x.Title, "Lucene.Net.Analysis.SimpleAnalyzer");
            Analyzers.Add(x => x.Desc, "Lucene.Net.Analysis.SimpleAnalyzer");
#endif
        }
    }
}
