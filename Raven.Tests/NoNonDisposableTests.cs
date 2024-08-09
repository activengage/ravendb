// -----------------------------------------------------------------------
//  <copyright file="NoNonDisposableTests.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Reflection;
using Raven35.Tests.Bugs.Identifiers;
using Raven35.Tests.Common.Attributes;

using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests
{
    public class NoNonDisposableTests : IDisposable
    {
        [Fact]
        public void ShouldExist()
        {
            var types = from test in typeof(NoNonDisposableTests).Assembly.GetTypes()
                        where test.GetMethods().Any(x => x.GetCustomAttributes(typeof(FactAttribute), true).Length != 0 ||
                                                         x.GetCustomAttributes(typeof(TheoryAttribute), true).Length != 0 ||
                                                         x.GetCustomAttributes(typeof(IISExpressInstalledFactAttribute), true)
                                                             .Length != 0 ||
                                                         x.GetCustomAttributes(typeof(IISExpressInstalledTheoryAttribute), true)
                                                             .Length != 0)
                        where typeof(IDisposable).IsAssignableFrom(test) == false
                        select test;

            var array = types.ToArray();
            if (array.Length == 0)
                return;

            Assert.True(false, string.Join(Environment.NewLine, array.Select(x => x.FullName)));
        }

        public void Dispose()
        {

        }
    }
}
