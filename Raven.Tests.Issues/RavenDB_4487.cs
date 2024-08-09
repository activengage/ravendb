// -----------------------------------------------------------------------
//  <copyright file="RavenDB_4487.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Net.Http;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_4487 : RavenTest
    {
        [Fact]
        public void CanFetchRoutes()
        {
            using (var store = NewRemoteDocumentStore())
            {

                using (var request = store.DatabaseCommands.CreateRequest("/debug/routes", HttpMethod.Get))
                {
                    var response = request.ReadResponseJson();
                    var jObject = response as RavenJObject;
                    Assert.NotNull(jObject);
                    Assert.True(jObject.Count > 100);
                }
            }
        }
    }
}