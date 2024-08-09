using System;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Tests.Common.Attributes;
using Raven35.Tests.Common.Util;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class IisQueryLengthIssues : IisExpressTestClient
    {
        private readonly string[] errorOptions = new[]
        {
            "configuration/system.webServer/security/requestFiltering/requestLimits@maxQueryString",
            "maxQueryStringLength"
        };

        [IISExpressInstalledFact]
        public void ShouldNotFail_DynamicIndex()
        {
            using (var store = NewDocumentStore())
            {
                var name = new string('x', 0x1000);
                store.OpenSession().Query<User>().Where(u => u.FirstName == name).ToList();
            }
        }

        [IISExpressInstalledFact]
        public void ShouldNotFail_StaticIndex()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = "from u in docs.Users select new { u.FirstName };"
                });
                var name = new string('x', 0x1000);
                store.OpenSession().Query<User>("test").Where(u => u.FirstName == name).ToList();
            }
        }
    }
}
