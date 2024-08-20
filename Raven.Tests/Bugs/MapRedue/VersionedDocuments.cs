using System;
using System.Linq;
using Raven35.Abstractions;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;

namespace Raven35.Tests.Bugs.MapRedue
{
    /// <summary>
    /// Index definition containing only last version of each document
    /// </summary>
    public class VersionedDocuments : AbstractIndexCreationTask<Document, DocumentView>
    {
        public VersionedDocuments()
        {
            Map = aDocuments =>
                  from document in aDocuments
                  from version in document.Versions
                  select new
                  {
                    document.Id,
                    version.Version,
                    Document = version
                  };
            Reduce = aResults =>
                     from result in aResults
                     group result by result.Id
                     into g
                     select new
                     {
                        Id = g.Key,
                        g.Where(aView => aView.Version ==
                                         g.Max(aView2 => aView2.Version)).FirstOrDefault().Version,
                        g.Where(aView => aView.Version ==
                                         g.Max(aView2 => aView2.Version)).FirstOrDefault().Document
                     };

            Store(x => x.Version, FieldStorage.Yes);
            Store(x => x.Document, FieldStorage.Yes);
        }
    }
}
