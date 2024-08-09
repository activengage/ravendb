// -----------------------------------------------------------------------
//  <copyright file="EtagUsage.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Exceptions;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class EtagUsage : RavenTest
    {
        [Fact]
        public void TryingToOverwriteItemWithNewOneWouldFail()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.Put("users/1", null, new RavenJObject(), new RavenJObject());

                using (var session = store.OpenSession())
                {
                    session.Store(new User());
                    Assert.Throws<ConcurrencyException>(() => session.SaveChanges());
                }
            }
        }
    }
}
