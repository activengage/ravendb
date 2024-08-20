// -----------------------------------------------------------------------
//  <copyright file="DatabaseConfiguration.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class DatabaseConfiguration : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            using (var store = NewDocumentStore())
            {
                var configuration = store.DatabaseCommands.Admin.GetDatabaseConfiguration();
                Assert.NotNull(configuration);

                configuration = store.DatabaseCommands.ForSystemDatabase().Admin.GetDatabaseConfiguration();
                Assert.NotNull(configuration);
            }
        }
    }
}
