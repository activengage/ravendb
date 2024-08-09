using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Util;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Linq;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Analyzers;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class NGramSearch : RavenTest
    {
        public class Image
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public ICollection<string> Users { get; set; }
            public ICollection<string> Tags { get; set; }
        }



        [Fact]
        public void Can_search_inner_words()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new FullTextSearchOnTags.Image { Id = "1", Name = "Great Photo buddy" });
                    session.Store(new FullTextSearchOnTags.Image { Id = "2", Name = "Nice Photo of the sky" });
                    session.SaveChanges();
                }

                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs.Images select new { doc.Name }",
                    Indexes =
                                                            {
                                                                {"Name", FieldIndexing.Analyzed}
                                                            },
                    Analyzers =
                                                            {
                                                                {"Name", typeof (NGramAnalyzer).AssemblyQualifiedName}
                                                            }
                });

                using (var session = store.OpenSession())
                {
                    var images = session.Query<FullTextSearchOnTags.Image>("test")
                        .Customize(x => x.WaitForNonStaleResults())
                        .OrderBy(x => x.Name)
                        .Search(x => x.Name, "phot")
                        .ToList();
                    Assert.NotEmpty(images);
                }
            }
        }

    }
}
