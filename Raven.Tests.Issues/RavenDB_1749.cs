// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1749.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Net;

using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_1749 : ReplicationBase
    {
        [Fact]
        public void PassingInvalidDocEtagDoesNotIgnoreAttachmentEtagWhenPurgingTombstones()
        {
            var store1 = CreateStore(databaseName: Constants.SystemDatabase);

            store1.DatabaseCommands.PutAttachment("attachment/1", null, new MemoryStream(), new RavenJObject());
            store1.DatabaseCommands.DeleteAttachment("attachment/1", null);

            servers[0].SystemDatabase.TransactionalStorage.Batch(accessor => Assert.NotEmpty(accessor.Lists.Read(Constants.RavenReplicationAttachmentsTombstones, Etag.Empty, null, 10)));

            Etag lastAttachmentEtag = Etag.Empty.Setup(UuidType.Attachments, 1).IncrementBy(3);

            var createHttpJsonRequestParams = new CreateHttpJsonRequestParams(null,
                                                                              servers[0].SystemDatabase.ServerUrl +
                                                                              string.Format("admin/replication/purge-tombstones?docEtag={0}&attachmentEtag={1}", null, lastAttachmentEtag),
                                                                              HttpMethods.Post,
                                                                              new OperationCredentials(null, CredentialCache.DefaultCredentials),
                                                                              store1.Conventions);
            store1.JsonRequestFactory.CreateHttpJsonRequest(createHttpJsonRequestParams).ExecuteRequest();


            servers[0].SystemDatabase.TransactionalStorage.Batch(accessor => Assert.Empty(accessor.Lists.Read(Constants.RavenReplicationAttachmentsTombstones, Etag.Empty, null, 10)));
        }
    }
}
