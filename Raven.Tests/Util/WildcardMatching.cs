// -----------------------------------------------------------------------
//  <copyright file="WildcardMatching.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Raven35.Abstractions.Util;
using Raven35.Database.Util;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Attributes;

using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests.Util
{
    public class WildcardMatching : NoDisposalNeeded
    {
        [Theory]
        [InlineValue("ay?nde", "ayende", true)]
        [InlineValue("ay?nde", "ayend", false)]
        [InlineValue("rav*b", "RavenDB", true)]
        [InlineValue("rav*b", "raven", false)]
        [InlineValue("*orders*", "customers/1/orders/123", true)]
        [InlineValue("*orders", "customers/1/orders", true)]
        [InlineValue("*orders|*invoices", "customers/1/orders", true)]
        [InlineValue("*orders|*invoices", "customers/1/invoices", true)]
        [InlineValue("*orders||*invoices", "customers/1/invoices", true)]
        public void CanMatch(string pattern, string input, bool expected)
        {
            Assert.Equal(expected, WildcardMatcher.Matches(pattern, input));
        }
    }
}
