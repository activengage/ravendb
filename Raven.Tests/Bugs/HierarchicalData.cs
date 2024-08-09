//-----------------------------------------------------------------------
// <copyright file="HierarchicalData.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Threading;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Embedded;
using Raven35.Json.Linq;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using Xunit;

namespace Raven35.Tests.Bugs
{
    public class HierarchicalData : RavenTest
    {
        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;

        public HierarchicalData()
        {
            store = NewDocumentStore();
            db = store.SystemDatabase;
        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanCreateHierarchicalIndexes()
        {
            db.Indexes.PutIndex("test", new IndexDefinition
            {
                Map = @"
from post in docs.Posts
from comment in Recurse(post, ((Func<dynamic,dynamic>)(x=>x.Comments)))
select new { comment.Text }"
            });

            db.Documents.Put("abc", null, RavenJObject.Parse(@"
{
    'Name': 'Hello Raven',
    'Comments': [
        { 'Author': 'Ayende', 'Text': 'def',	'Comments': [ { 'Author': 'Rahien', 'Text': 'abc' } ] }
    ]
}
"), RavenJObject.Parse("{'Raven-Entity-Name': 'Posts'}"), null);

            QueryResult queryResult;
            do
            {
                queryResult = db.Queries.Query("test", new IndexQuery
                {
                    Query = "Text:abc"
                }, CancellationToken.None);
            } while (queryResult.IsStale);

            Assert.Equal(1, queryResult.Results.Count);
        }
    }
}
