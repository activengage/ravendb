using Raven35.Abstractions.Exceptions;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Tests.Common;
using Xunit;
using Xunit.Extensions;

namespace Raven35.Tests.Bugs
{
    public class BulkInsert : RavenTest
    {
        [Theory]
        [PropertyData("Storages")]
        public void bulk_insert_with_duplicates(string storageName)
        {
            using (GetNewServer(requestedStorage: storageName))
            using (var store = new DocumentStore() { Url = "http://localhost:8079" }.Initialize())
            {
                CreateBulk(store);

                //should throw a ConcurrencyException
                var e = Assert.Throws<ConcurrencyException>(() => CreateBulk(store));
                Assert.Contains(@"ConcurrencyException while writing bulk insert items in the server.", e.Message);
            }
        }

        private static void CreateBulk(IDocumentStore store)
        {
            using (var bulkInsert = store.BulkInsert())
            {
                for (var i = 0; i < 2000; i++)
                {
                    bulkInsert.Store(new Test {Id = "" + i});
                }
            }
        }

        private class Test
        {
            public string Id { get; set; }
        }
    }
}
