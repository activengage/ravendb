using System;
using System.Threading;
using System.Transactions;
using Raven35.Json.Linq;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;
using TransactionInformation = Raven35.Abstractions.Data.TransactionInformation;

namespace Raven35.Tests.Bugs.DTC
{
    public class UsingDtcForDelete : RavenTest
    {
        private string documentKey;

        [Fact]
        public void ShouldWork()
        {
            using (var store = NewDocumentStore(requestedStorage: "esent"))
            {
                EnsureDtcIsSupported(store);
                documentKey = "tester123";

                var transactionInformation = new TransactionInformation
                {
                    Id = Guid.NewGuid().ToString()
                };

                store.SystemDatabase.Documents.Put(documentKey, null, new RavenJObject(),
                                     RavenJObject.Parse(
                                        @"{
  ""Raven-Entity-Name"": ""MySagaDatas"",
  ""Raven-Clr-Type"": ""TestNServiceBusSagaWithRavenDb.MySagaData, TestNServiceBusSagaWithRavenDb"",
  ""Last-Modified"": ""Mon, 21 Mar 2011 19:59:58 GMT"",
  ""Non-Authoritative-Information"": false
}"), transactionInformation);
                store.DatabaseCommands.PrepareTransaction(transactionInformation.Id);
                store.DatabaseCommands.Commit(transactionInformation.Id);


                var deleteTx = new TransactionInformation
                {
                    Id = Guid.NewGuid().ToString()
                };
                store.SystemDatabase.Documents.Delete(documentKey, null, deleteTx);

                store.DatabaseCommands.PrepareTransaction(deleteTx.Id);
                store.SystemDatabase.Commit(deleteTx.Id);
            }
        }
    }
}
