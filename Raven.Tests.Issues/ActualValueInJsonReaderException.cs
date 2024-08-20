using System;
using System.Net.Http;
using System.Threading.Tasks;

using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Extensions;
using Raven35.Database.Server.WebApi;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class ActualValueInJsonReaderException : RavenTest
    {
        [Fact]
        public async Task JsonErrorsShouldIncludeOriginalData()
        {
            var responseMesage = new HttpResponseMessage
            {
                ReasonPhrase = "<>,./:",
                Content = new MultiGetSafeStringContent("<>,./:")
            };

            var responseException = ErrorResponseException.FromResponseMessage(responseMesage, true);
            var exception = await AssertAsync.Throws<InvalidOperationException>(async () => await responseException.TryReadErrorResponseObject<string>());
            Assert.Contains("Exception occured reading the string: ", exception.Message);
        }
    }
}
