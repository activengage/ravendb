// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1877.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using Raven35.Abstractions.Spatial;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_1877 : NoDisposalNeeded
    {
        [Fact]
         public void CanReadShapeFromListOfDecimal()
         {
            var list = new List<decimal> { 5.7m, 9.2m };
            var shapeConvert = new ShapeConverter();
            string result;
            Assert.True(shapeConvert.TryConvert(list, out result));
            Assert.Equal("POINT (5.7 9.2)", result);

         }
    }
}
