using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven35.Client.Document;
using Raven35.Json.Linq;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3691 : RavenTestBase
    {
        [Fact]
        public void CanPutDocumentWithMetadataPropertyBeingNull()
        {
            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    documentStore.DatabaseCommands.Put("test", null, new RavenJObject(), RavenJObject.FromObject(new { Foo = (string)null }));
                }
            }
        }

        [Fact]
        public void CanPutAttachmentWithMetadataPropertyBeingNull()
        {
            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    documentStore.DatabaseCommands.PutAttachment("test", null, new MemoryStream(new byte[] { 1, 2, 3, 4 }), RavenJObject.FromObject(new { Foo = (string)null }));
                }
            }
        }
    }
}
