//-----------------------------------------------------------------------
// <copyright file="LastModifiedRemote.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven35.Abstractions;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Metadata
{
    public class LastModifiedRemote : RavenTest
    {
        [Fact]
        public void CanAccessLastModifiedAsMetadata()
        {
            using(GetNewServer())
            using (var store = new DocumentStore{Url = "http://localhost:8079"}.Initialize())
            {
                DateTime before;
                DateTime after;

                using (var session = store.OpenSession())
                {
                    session.Store(new User());

                    before = SystemTime.UtcNow;
                    session.SaveChanges();
                    after = SystemTime.UtcNow;
                }

                using (var session = store.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    var lastModified = session.Advanced.GetMetadataFor(user).Value<DateTime>("Last-Modified");
                    Assert.NotNull(lastModified);
                    int msPrecision = 1000;
                    Assert.InRange(lastModified, before.AddMilliseconds(-msPrecision), after.AddMilliseconds(msPrecision));
                    Assert.Equal(DateTimeKind.Utc, lastModified.Kind);
                }
            }
        }
    }
}
