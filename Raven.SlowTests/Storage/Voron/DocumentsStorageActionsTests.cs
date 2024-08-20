using System;
using System.Text;

using Raven35.Abstractions.Data;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;
using Xunit.Extensions;

namespace Raven35.SlowTests.Storage.Voron
{
    [Trait("VoronTest", "StorageActionsTests")]
    [Trait("VoronTest", "DocumentStorage")]
    public class DocumentsStorageActionsTests : TransactionalStorageTestBase
    {
        [Theory]
        [PropertyData("Storages")]
        public void DocumentStorage_Massive_AddDocuments_DeleteDocuments_No_Errors(string storageName)
        {
            const int DOCUMENT_COUNT = 750;
            var rand = new Random();
            var testBuffer = new byte[500];
            rand.NextBytes(testBuffer);
            var testString = Encoding.Unicode.GetString(testBuffer);
            for (int i = 0; i < 50; i++)
            {
                using (var storage = NewTransactionalStorage(storageName))
                {
                    storage.Batch(mutator =>
                    {
                        for (int docIndex = 0; docIndex < DOCUMENT_COUNT; docIndex++)
                            mutator.Documents.AddDocument("Foo" + docIndex, null, RavenJObject.FromObject(new { Name = testString }),
                                new RavenJObject());
                    });

                    storage.Batch(mutator =>
                    {
                        for (var docIndex = 0; docIndex < DOCUMENT_COUNT; docIndex++)
                        {
                            Etag deletedEtag;
                            RavenJObject metadata;
                            mutator.Documents.DeleteDocument("Foo" + docIndex, null, out metadata, out deletedEtag);
                        }
                    });
                }
            }
        }

    }
}
