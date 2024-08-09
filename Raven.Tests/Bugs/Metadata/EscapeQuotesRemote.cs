//-----------------------------------------------------------------------
// <copyright file="LastModifiedRemote.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Metadata
{
    public class EscapeQuotesRemote : RavenTest
    {
        [Fact]
        public void CanProperlyEscapeQuotesInMetadata_Remote_1()
        {
            using (GetNewServer())
            using (var store = new DocumentStore { Url = "http://localhost:8079" }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    var user = new User();
                    session.Store(user);
                    session.Advanced.GetMetadataFor(user).Add("Foo", "\"Bar\"");
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    var metadata = session.Advanced.GetMetadataFor(user);
                    Assert.Equal("\"Bar\"", metadata.Value<string>("Foo"));
                }
            }
        }

        [Fact]
        public void CanProperlyEscapeQuotesInMetadata_Remote_2()
        {
            using (GetNewServer())
            using (var store = new DocumentStore { Url = "http://localhost:8079" }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    var user = new User();
                    session.Store(user);
                    session.Advanced.GetMetadataFor(user).Add("Foo", "\\\"Bar\\\"");
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var user = session.Load<User>("users/1");
                    var metadata = session.Advanced.GetMetadataFor(user);
                    Assert.Equal("\\\"Bar\\\"", metadata.Value<string>("Foo"));
                }
            }
        }
    }
}
