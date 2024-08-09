using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Client.Document;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class Attachments : RavenTest
    {
        [Fact]
        public void CanHeadExistingAttachment()
        {
            Attachment attachment;

            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    documentStore.DatabaseCommands.PutAttachment("test", null, new MemoryStream(new byte[] { 1, 2, 3, 4 }), new RavenJObject());
                    attachment = documentStore.DatabaseCommands.HeadAttachment("test");
                }
            }

            Assert.NotNull(attachment);
            var exception = Assert.Throws<InvalidOperationException>(() => attachment.Data());
            Assert.Equal("Cannot get attachment data because it was loaded using: HEAD", exception.Message);
        }

        [Fact]
        public void CanHeadNonExistingAttachment()
        {
            Attachment attachment;

            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    attachment = documentStore.DatabaseCommands.HeadAttachment("test");
                }
            }

            Assert.Null(attachment);
        }

        [Fact]
        public async Task CanHeadExistingAttachmentAsync()
        {
            Attachment attachment;

            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    documentStore.DatabaseCommands.PutAttachment("test", null, new MemoryStream(new byte[] { 1, 2, 3, 4 }), new RavenJObject());
                    attachment = await documentStore.AsyncDatabaseCommands.HeadAttachmentAsync("test");
                }
            }

            Assert.NotNull(attachment);
            Assert.Throws<InvalidOperationException>(() => attachment.Data());
        }

        [Fact]
        public async Task CanHeadNonExistingAttachmentAsync()
        {
            Attachment attachment;

            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    attachment = await documentStore.AsyncDatabaseCommands.HeadAttachmentAsync("test");
                }
            }

            Assert.Null(attachment);
        }

        [Fact]
        public void CanExportAttachments()
        {
            using (var server = GetNewServer())
            {
                using (var documentStore = new DocumentStore { Url = server.SystemDatabase.Configuration.ServerUrl }.Initialize())
                {
                    documentStore.DatabaseCommands.PutAttachment("test", null, new MemoryStream(new byte[] { 1, 2, 3, 4 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test2", Raven35.Abstractions.Data.Etag.InvalidEtag, new MemoryStream(new byte[] { 1, 2, 3, 5 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test3", Raven35.Abstractions.Data.Etag.InvalidEtag, new MemoryStream(new byte[] { 1, 2, 3, 4 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test4", Raven35.Abstractions.Data.Etag.InvalidEtag, new MemoryStream(new byte[] { 1, 2, 3, 5 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test5", Raven35.Abstractions.Data.Etag.InvalidEtag, new MemoryStream(new byte[] { 1, 2, 3, 4 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test6", null, new MemoryStream(new byte[] { 1, 2, 3, 5 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test7", Raven35.Abstractions.Data.Etag.InvalidEtag, new MemoryStream(new byte[] { 1, 2, 3, 4 }), new RavenJObject());
                    documentStore.DatabaseCommands.PutAttachment("test8", Raven35.Abstractions.Data.Etag.InvalidEtag, new MemoryStream(new byte[] { 1, 2, 3, 5 }), new RavenJObject());
                }

                using (var webClient = new WebClient())
                {
                    webClient.UseDefaultCredentials = true;
                    webClient.Credentials = CredentialCache.DefaultNetworkCredentials;

                    var lastEtag = Raven35.Abstractions.Data.Etag.Empty;
                    int totalCount = 0;
                    while (true)
                    {
                        var attachmentInfo =
                            GetString(webClient.DownloadData(server.SystemDatabase.Configuration.ServerUrl + "/static/?pageSize=2&etag=" + lastEtag));
                        var array = RavenJArray.Parse(attachmentInfo);

                        if (array.Length == 0) break;

                        totalCount += array.Length;

                        lastEtag = Raven35.Abstractions.Data.Etag.Parse(array.Last().Value<string>("Etag"));
                    }
                }
            }
        }

        public static string GetString(byte[] downloadData)
        {
            var ms = new MemoryStream(downloadData);
            return new StreamReader(ms, Encoding.UTF8).ReadToEnd();
        }
    }
}
