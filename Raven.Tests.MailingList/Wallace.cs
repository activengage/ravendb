// -----------------------------------------------------------------------
//  <copyright file="Wallace.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;
using Raven35.Client.Linq;
using System.Linq;

namespace Raven35.Tests.MailingList
{
    public class Wallace : RavenTest
    {
        [Fact]
        public void CanGetProperErrorFromComputedOrderBy()
        {
            using(GetNewServer())
            using(var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                using(var session = store.OpenSession())
                {
                    var argumentException = Assert.Throws<ArgumentException>(() => session.Query<Order>().OrderBy(x => x.OrderLines.Last().Quantity).ToList());

                    Assert.Equal("Could not understand expression: .OrderBy(x => x.OrderLines.Last().Quantity)", argumentException.Message);
                    Assert.Equal("Not supported computation: x.OrderLines.Last().Quantity. You cannot use computation in RavenDB queries (only simple member expressions are allowed).",
                        argumentException.InnerException.Message);
                }
            }
        }
    }
}
