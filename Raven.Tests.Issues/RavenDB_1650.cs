// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1650.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_1650 : RavenTest
    {
        public class User
        {
             
        }

        [Fact]
        public void ShouldProperlyDisposeEsentResourcesUsedByStreamingControllerWhenQuerying()
        {
            using (var store = NewRemoteDocumentStore(requestedStorage: "esent"))
            {
                using (var session = store.OpenSession())
                {
                    var enumerator = session.Advanced.Stream(session.Query<User>(new RavenDocumentsByEntityName().IndexName));
                    int count = 0;
                    while (enumerator.MoveNext())
                    {
                        count++;
                    }

                    Assert.Equal(0, count);
                }
            }
        }
    }
}
