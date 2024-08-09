//-----------------------------------------------------------------------
// <copyright file="AttachmentReadTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Raven35.Abstractions.Data;
using Raven35.Client.Embedded;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using System.Linq;
using Xunit;

namespace Raven35.Tests.Triggers
{
    public class AttachmentReadTrigger : RavenTest
    {
        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;

        public AttachmentReadTrigger()
        {
            store = NewDocumentStore(catalog:(new TypeCatalog(typeof (HideAttachmentByCaseReadTrigger))));
            db = store.SystemDatabase;
        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanFilterAttachment()
        {
            db.Attachments.PutStatic("ayendE", null, new MemoryStream(new byte[] { 1, 2 }), new RavenJObject());

            var attachment = db.Attachments.GetStatic("ayendE");

            Assert.Equal("You don't get to read this attachment",
                         attachment.Metadata.Value<RavenJObject>("Raven-Read-Veto").Value<string>("Reason"));
        }

        [Fact]
        public void CanHideAttachment()
        {
            db.Attachments.PutStatic("AYENDE", null, new MemoryStream(new byte[] { 1, 2 }), new RavenJObject());

            var attachment = db.Attachments.GetStatic("AYENDE");

            Assert.Null(attachment);
        }

        [Fact]
        public void CanModifyAttachment()
        {
            db.Attachments.PutStatic("ayende", null, new MemoryStream(new byte[] { 1, 2 }), new RavenJObject());


            var attachment = db.Attachments.GetStatic("ayende");

            Assert.Equal(attachment.Data().Length, 4);
        }

        public class HideAttachmentByCaseReadTrigger : AbstractAttachmentReadTrigger
        {
            public override ReadVetoResult AllowRead(string key, Stream data, RavenJObject metadata, ReadOperation operation)
            {
                if (key.All(char.IsUpper))
                    return ReadVetoResult.Ignore;
                if (key.Any(char.IsUpper))
                    return ReadVetoResult.Deny("You don't get to read this attachment");
                return ReadVetoResult.Allowed;
            }

            public override void OnRead(string key, Attachment attachment)
            {
                attachment.Data = () => new MemoryStream(new byte[] { 1, 2, 3, 4 });
                attachment.Size = 4;
            }
        }
    }
}
