// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2205.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;

namespace Raven35.Tests.Issues
{
    using System;
    using System.Globalization;

    using Raven35.Abstractions.Linq;

    using Xunit;

    public class RavenDB_2205 : NoDisposalNeeded
    {
        [Fact]
        public void DateToolsRoundShouldKeepTheSameDateTimeKind()
        {
            var originalDate = DateTime.Parse("2014-04-25T09:33:15.6457886Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            var roundedDate = DateTools.Round(originalDate, DateTools.Resolution.SECOND);

            var expectedTime = DateTime.Parse("2014-04-25T09:33:15Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            Assert.Equal(expectedTime, roundedDate);
            Assert.Equal(expectedTime.Kind, roundedDate.Kind);
        }
    }
}
