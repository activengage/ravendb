// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2495.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Bundles.MoreLikeThis;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2495 : RavenTest
    {
        [Fact]
        public void IncludesShouldWorkWithMoreLikeThis()
        {
            using (var x = new MoreLikeThisTests())
            {
                x.IncludesShouldWorkWithMoreLikeThis();
            }
        }
    }
}
