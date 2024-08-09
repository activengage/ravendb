//-----------------------------------------------------------------------
// <copyright file="AttachmentsWithCredentials.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using Raven35.Abstractions.Extensions;
using Raven35.Database.Config;
using Raven35.Database.Server;
using Raven35.Database.Server.Security;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class AttachmentsWithCredentials : RavenTest
    {
        protected override void ModifyConfiguration(InMemoryRavenConfiguration ravenConfiguration)
        {
            ravenConfiguration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            Authentication.EnableOnce();
        }

        [Fact]
        public void CanPutAndGetAttachmentWithAccessModeNone()
        {
            using (var store = NewRemoteDocumentStore())
            {
                store.DatabaseCommands.PutAttachment("ayende", null, new MemoryStream(new byte[] {1, 2, 3, 4}), new RavenJObject());
                Assert.Equal(new byte[] {1, 2, 3, 4}, store.DatabaseCommands.GetAttachment("ayende").Data().ReadData());
            }
        }

        [Fact]
        public void CanDeleteAttachmentWithAccessModeNone()
        {
            using (var store = NewRemoteDocumentStore())
            {
                store.DatabaseCommands.PutAttachment("ayende", null, new MemoryStream(new byte[] {1, 2, 3, 4}), new RavenJObject());
                Assert.Equal(new byte[] {1, 2, 3, 4}, store.DatabaseCommands.GetAttachment("ayende").Data().ReadData());

                store.DatabaseCommands.DeleteAttachment("ayende", null);
                Assert.Null(store.DatabaseCommands.GetAttachment("ayende"));
            }
        }
    }
}
