// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2205.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Net;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2706 : RavenTest
    {
        [Fact]
        public void HideTemporaryTransfomer()
        {
            using (var store = NewRemoteDocumentStore(databaseName:"db1"))
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User
                    {
                        Id = "users/1",
                        Name = "John"
                    });
                    session.SaveChanges();
                }

                var users = store.DatabaseCommands.Get("users/1");

                Assert.Equal(0, store.DatabaseCommands.GetTransformers(0, 32).Length);
                WaitForIndexing(store);
                
                  var createHttpJsonRequestParams = new CreateHttpJsonRequestParams(null,
                                                                              servers[0].SystemDatabase.ServerUrl +
                                                                              string.Format("databases/db1/streams/exploration?linq={0}&collection=Users&timeoutSeconds=30", "from result in results select result"),
                                                                              HttpMethods.Get,
                                                                              new OperationCredentials(null, CredentialCache.DefaultCredentials),
                                                                              store.Conventions);

                var json = store.JsonRequestFactory.CreateHttpJsonRequest(createHttpJsonRequestParams).ReadResponseJson();
                Assert.Equal(1, json.Value<RavenJArray>("Results").Length);
                Assert.Equal(0, store.DatabaseCommands.GetTransformers(0, 32).Length);
            }
        }
    }
}
