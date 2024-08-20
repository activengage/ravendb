//-----------------------------------------------------------------------
// <copyright file="TranslatingLinqQueryUsingNestedId.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class TranslatingLinqQueryUsingNestedId : RavenTest
    {
        [Fact]
        public void Id_on_member_should_not_be_converted_to_document_id()
        {
            var generated = new IndexDefinitionBuilder<SubCategory>()
            {
                Map = subs => from subCategory in subs
                              select new
                              {
                                  CategoryId = subCategory.Id,
                                  SubCategoryId = subCategory.Parent.Id
                              }
            }.ToIndexDefinition(new DocumentConvention());
            
            Assert.Contains("CategoryId = subCategory.__document_id", generated.Map);
            Assert.Contains("SubCategoryId = subCategory.Parent.Id", generated.Map);
        }

        #region Nested type: Category

        public class Category
        {
            public string Id { get; set; }
        }

        #endregion

        #region Nested type: SubCategory

        public class SubCategory
        {
            public string Id { get; set; }
            public Category Parent { get; set; }
        }

        #endregion
    }
}
