using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3509 : RavenTestBase
    {
        [Fact]
        public void WaitForUserToContinueTheTestValidate()
        {
            using (var store = NewRemoteDocumentStore(fiddler: true))
            {
                store.Initialize();
                WaitForUserToContinueTheTest();
            }
        }

        [Fact]
        public void WaitForUserToContinueTheTestValidateServerConnection()
        {
            using (var store = NewDocumentStore())
            {
                store.Initialize();
                
                var e = Assert.Throws<NotSupportedException>(() => WaitForUserToContinueTheTest(debug: false));
                Assert.Contains("when using a local store WaitForUserToContinueTheTest must be called with store parameter", e.Message);

            }
        }
    }
}
