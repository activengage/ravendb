using Raven35.Abstractions.Linq;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class StringIsNullOrEmpty : NoDisposalNeeded
    {
        [Fact]
        public void ShouldWork()
        {
            dynamic doc = new DynamicJsonObject(new RavenJObject());

            Assert.True(string.IsNullOrEmpty(doc.Name));
        } 
    }
}
