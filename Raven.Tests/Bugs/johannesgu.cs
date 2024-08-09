using System;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Exceptions;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class johannesgu : RavenTest
    {
         [Fact]
         public void FailureToCommitDocWithSlashInIt()
         {
             using(var store = NewDocumentStore())
             {
                 var tx = new TransactionInformation
                 {
                     Id = Guid.NewGuid().ToString(),
                     Timeout = TimeSpan.FromMinutes(1)
                 };

                 Assert.Throws<OperationVetoedException>(
                     () => store.SystemDatabase.Documents.Put(@"somebadid\123", null, new RavenJObject(), new RavenJObject(), tx));
             }
         }
    }
}
