// -----------------------------------------------------------------------
//  <copyright file="HttpIdTest.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;
using Raven35.Tests.Helpers;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class HttpIdTest : RavenTestBase
    {
        [Fact]
        public void CanLoadIdWithHttp()
        {
            using (var store = NewRemoteDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new Foo { Id = "http://whatever" });
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var foo = session.Load<Foo>("http://whatever");
                    Assert.NotNull(foo);
                }
            }
        }

        public class Foo
        {
            public string Id { get; set; }
        }
    }
}
