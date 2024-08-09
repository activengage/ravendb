using System;
using System.IO;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.Bugs
{
    public class EmptyAttachments : RavenTest
    {
        [Fact]
        public void CanSaveAndLoad()
        {
            using (var store = NewDocumentStore(requestedStorage: "esent"))
            {
                store.DatabaseCommands.PutAttachment("a", null, new MemoryStream(), new RavenJObject());

                var attachment = store.DatabaseCommands.GetAttachment("a");

                Assert.Equal(0, attachment.Data().Length);
            }
        }

        [Fact]
        public void CanSaveAndIterate()
        {
            using (var store = NewDocumentStore(requestedStorage: "esent"))
            {
                store.DatabaseCommands.PutAttachment("a", null, new MemoryStream(), new RavenJObject());

                store.SystemDatabase.TransactionalStorage.Batch(accessor =>
                {
                    accessor.Attachments.GetAttachmentsAfter(Raven35.Abstractions.Data.Etag.Empty, 100, long.MaxValue).ToList();
                });
            }
        }
    }
}
