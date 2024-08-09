// -----------------------------------------------------------------------
//  <copyright file="JsonComarision.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Json.Linq;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class JsonComarision : NoDisposalNeeded
    {
        [Fact]
        public void ShouldNotThrow()
        {
            var obj1 = new RavenJObject();
            obj1["Raven-Replication-Version"] = null;
            obj1["Raven-Replication-Source"] = null;

            var obj2 = new RavenJObject();
            obj2["Raven-Replication-Version"] = null;
            obj2["Raven-Replication-Source"] = "http://someserver";

            Assert.False(RavenJTokenEqualityComparer.Default.Equals(obj1, obj2));
        } 
    }
}
