// -----------------------------------------------------------------------
//  <copyright file="ProjectionShouldNotLoadDocument.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class ProjectionShouldNotLoadDocument : RavenTest
    {
        [Fact]
        public void WhenProjecting()
        {
            using (var store = NewDocumentStore())
            {
                store.SystemDatabase.Documents.Put("FOO", null, new RavenJObject { { "Name", "Ayende" } }, new RavenJObject(), null);
                WaitForIndexing(store);
                var result = store.DatabaseCommands.Query("dynamic", new IndexQuery
                 {
                     FieldsToFetch = new[] { "Name" }
                 }, new string[0]);

                // if this is lower case, then we loaded this from the index, not from the db
                Assert.Equal("foo", result.Results[0].Value<string>(Constants.DocumentIdFieldName));
            }
        }
    }
}
