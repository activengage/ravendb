// -----------------------------------------------------------------------
//  <copyright file="RDBQA_18.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RDBQA_18 : RavenTest
    {
        [Fact]
        public void ShouldNotThrowNullReferenceException()
        {
            using (var store = new DocumentStore())
            {
                Assert.DoesNotThrow(store.Replication.WaitAsync().Wait);
            }
        }
    }
}
