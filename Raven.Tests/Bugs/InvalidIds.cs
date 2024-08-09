//-----------------------------------------------------------------------
// <copyright file="InvalidIds.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class InvalidIds : RavenTest
    {
        [Fact]
        public void DocumentIdCannotStartWithSlash()
        {
            using(var store = NewDocumentStore())
            {
                using(var s = store.OpenSession())
                {
                    var invalidOperationException = Assert.Throws<InvalidOperationException>(()=>s.Store(new {Id = "/hello"}));
                    Assert.Equal("Cannot use value '/hello' as a document id because it begins with a '/'", invalidOperationException.Message);
                }
            }
        }
    }
}
