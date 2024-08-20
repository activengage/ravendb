using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class TomCabanski : RavenTest
    {
        [Fact]
        public void CanEscapeGetFacets()
        {
            using (GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.Age, doc.IsActive, doc.BookVendor }"
                });

                using (var s = store.OpenSession())
                {
                    s.Store(new FacetSetup
                    {
                        Id = "facets/test",
                        Facets =
                            {
                                new Facet
                                {
                                    Mode = FacetMode.Default,
                                    Name = "Age"
                                }
                            }
                    });
                    s.SaveChanges();
                }

                store.DatabaseCommands.GetFacets("test", new IndexQuery
                {
                    Query = "(IsActive:true)  AND (BookVendor:\"stroheim & romann\")"
                }, "facets/test");
            }
        }
    }
}
