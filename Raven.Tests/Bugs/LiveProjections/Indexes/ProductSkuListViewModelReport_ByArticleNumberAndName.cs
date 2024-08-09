using Raven35.Abstractions.Indexing;

namespace Raven35.Tests.Bugs.LiveProjections.Indexes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Raven35.Client.Indexes;
    using Raven35.Database.Indexing;
    using Raven35.Tests.Bugs.LiveProjections.Entities;
    using Raven35.Tests.Bugs.LiveProjections.Views;

    public class ProductSkuListViewModelReport_ByArticleNumberAndName : AbstractIndexCreationTask<ProductSku>
    {
        public ProductSkuListViewModelReport_ByArticleNumberAndName()
        {
            Map = products => from product in products
                              select new
                                {
                                    Id = product.Id,
                                    ArticleNumber = product.ArticleNumber,
                                    Name = product.Name
                                };


            Indexes = new Dictionary<Expression<Func<ProductSku, object>>, FieldIndexing>()
                {
                    { e=>e.ArticleNumber, FieldIndexing.Analyzed},
                    { e=>e.Name, FieldIndexing.Analyzed}
                };
        }
    }

    public class ProductSkuListViewModelReport_ByArticleNumberAndNameTransformer : AbstractTransformerCreationTask<ProductSku>
    {
        public ProductSkuListViewModelReport_ByArticleNumberAndNameTransformer()
        {
            TransformResults = results =>
                               from result in results
                               let product = LoadDocument<ProductSku>(result.Id)
                               let stock = LoadDocument<ProductSku>(result.Id)
                               select new
                               {
                                   result.Id,
                                   result.ArticleNumber,
                                   result.Name,
                                   product.Packing,
                                   stock.QuantityInWarehouse
                               };
        }
    }
}
