using System.Diagnostics;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Tests.Common;

namespace Raven35.Tests.Bugs.LiveProjections
{
    using System.Collections.Generic;
    using System.Linq;

    using Raven35.Client.Linq;
    using Raven35.Tests.Bugs.LiveProjections.Entities;
    using Raven35.Tests.Bugs.LiveProjections.Indexes;
    using Raven35.Tests.Bugs.LiveProjections.Views;

    using Xunit;

    public class LiveProjectionOnProducts : RavenTest
    {
        [Fact]
        public void SimpleLiveProjection()
        {
            using (var documentStore = NewDocumentStore())
            {
                new ProductSkuListViewModelReport_ByArticleNumberAndName().Execute(((IDocumentStore) documentStore).DatabaseCommands, ((IDocumentStore) documentStore).Conventions);
                new ProductSkuListViewModelReport_ByArticleNumberAndNameTransformer().Execute(documentStore);

                using (var session = documentStore.OpenSession())
                {
                    session.Store(
                        new ProductSku()
                            {
                                ArticleNumber = "v1",
                                Name = "variant 1",
                                Packing = "packing",
                            });
                    session.Store(
                        new ProductSku()
                            {
                                ArticleNumber = "v2",
                                Name = "variant 2",
                                Packing = "packing"
                            });
                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var rep = session.Query<dynamic, ProductSkuListViewModelReport_ByArticleNumberAndName>()
                        .Customize(x =>
                        {
                            x.WaitForNonStaleResultsAsOfNow();
                            ((IDocumentQuery<ProductSkuListViewModelReport>)x).OrderBy("__document_id");
                        })
                        .TransformWith<ProductSkuListViewModelReport_ByArticleNumberAndNameTransformer, ProductSkuListViewModelReport>()
                        .ToList();

                    var first = rep.FirstOrDefault();

                    Assert.Equal(first.Id, "productskus/1");
                    Assert.Equal(first.Name, "variant 1");
                }
            }
        }

        [Fact]
        public void ComplexLiveProjection()
        {
            using (var documentStore = NewDocumentStore())
            {
                new ProductDetailsReport_ByProductId().Execute(((IDocumentStore) documentStore).DatabaseCommands, ((IDocumentStore) documentStore).Conventions);
                new ProductDetailsReport_Transformer().Execute(documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var product = new Product()
                        {
                            Name = "product 1",
                            Variants = new List<ProductSku>()
                                {
                                    new ProductSku()
                                        {
                                            ArticleNumber = "v1",
                                            Name = "variant 1",
                                            Packing = "packing"
                                        },
                                    new ProductSku()
                                        {
                                            ArticleNumber = "v2",
                                            Name = "variant 2",
                                            Packing = "packing"
                                        }
                                }
                        };

                    session.Store(product);
                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var rep = session.Query<dynamic, ProductDetailsReport_ByProductId>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .TransformWith<ProductDetailsReport_Transformer, ProductDetailsReport>()
                        .ToList();

                    var first = rep.FirstOrDefault();

                    Assert.Equal(first.Name, "product 1");
                    Assert.Equal(first.Id, "products/1");
                    Assert.Equal(first.Variants[0].Name, "variant 1");
                }
            }
        }
    }
}
