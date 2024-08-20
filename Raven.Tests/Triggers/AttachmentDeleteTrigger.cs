//-----------------------------------------------------------------------
// <copyright file="AttachmentDeleteTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Raven35.Client.Embedded;
using Raven35.Json.Linq;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Abstractions.Exceptions;
using Raven35.Database.Plugins;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using Xunit;

namespace Raven35.Tests.Triggers
{
    public class AttachmentDeleteTrigger: RavenTest
    {
        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;

        public AttachmentDeleteTrigger()
        {
            store = NewDocumentStore(catalog:(new TypeCatalog(typeof (RefuseAttachmentDeleteTrigger))));
            db = store.SystemDatabase;
        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanVetoDeletes()
        {
            db.Attachments.PutStatic("ayende", null, new MemoryStream(new byte[] { 1, 2, 3 }), new RavenJObject());
            var operationVetoedException = Assert.Throws<OperationVetoedException>(()=>db.Attachments.DeleteStatic("ayende", null));
            Assert.Equal("DELETE vetoed on attachment ayende by Raven35.Tests.Triggers.AttachmentDeleteTrigger+RefuseAttachmentDeleteTrigger because: Can't delete attachments", operationVetoedException.Message);
        }

        public class RefuseAttachmentDeleteTrigger: AbstractAttachmentDeleteTrigger
        {
            public override VetoResult AllowDelete(string key)
            {
                return VetoResult.Deny("Can't delete attachments");
            }
        }
    }
}
