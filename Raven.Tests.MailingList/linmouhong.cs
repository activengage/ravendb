// -----------------------------------------------------------------------
//  <copyright file="linmouhong.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;

using Xunit;
using Raven35.Client.Linq;

namespace Raven35.Tests.MailingList
{
    public class linmouhong : RavenTest
    {
        public class Item
        {
            public Product Product;
        }

        public class Product
        {
            public string Name;
        }
        
        [Fact]
        public void CanCreateProperNestedQuery()
        {
            using(var store = NewDocumentStore())
            {
                using(var session = store.OpenSession())
                {
                    var s = session.Advanced.DocumentQuery<Item>("test").WhereEquals(x => x.Product.Name, "test").ToString();
                    
                    Assert.Equal("Product_Name:test", s);
                    s = session.Advanced.DocumentQuery<Item>().WhereEquals(x => x.Product.Name, "test").ToString();

                    Assert.Equal("Product.Name:test", s);
                }
            }
        }
    }
}
