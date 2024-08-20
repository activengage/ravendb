// -----------------------------------------------------------------------
//  <copyright file="InflectorTests.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Client.Util;
using Raven35.Tests.Common;

using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests
{
    public class InflectorTests : NoDisposalNeeded
    {
        [Theory]
        [InlineData("User", "Users")]
        [InlineData("Users", "Users")]
        [InlineData("tanimport", "tanimports")]
        [InlineData("tanimports", "tanimports")]
        public void CanUsePluralizeSafelyOnMaybeAlreadyPluralizedWords(string word, string expected)
        {
            Assert.Equal(expected, Inflector.Pluralize(word));
        }
    }
}
