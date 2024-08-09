//-----------------------------------------------------------------------
// <copyright file="MapReduce_IndependentSteps.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Threading;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Json.Linq;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using Xunit;
using Raven35.Client.Embedded;

namespace Raven35.Tests.Views
{
    public class MapReduce_IndependentSteps : RavenTest
    {
        private const string map =
            @"from post in docs
select new {
  post.blog_id, 
  comments_length = post.comments.Length 
  }";

        private const string reduce =
            @"
from agg in results
group agg by agg.blog_id into g
select new { 
  blog_id = g.Key, 
  comments_length = g.Sum(x=>(int)x.comments_length)
  }";

        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;

        public MapReduce_IndependentSteps()
        {
            store = NewDocumentStore();
            db = store.SystemDatabase;
            db.Indexes.PutIndex("CommentsCountPerBlog", new IndexDefinition{Map = map, Reduce = reduce, Indexes = {{"blog_id", FieldIndexing.NotAnalyzed}}});
        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanGetReducedValues()
        {
            var values = new[]
            {
                "{blog_id: 3, comments: [{},{},{}]}",
                "{blog_id: 5, comments: [{},{},{},{}]}",
                "{blog_id: 6, comments: [{},{},{},{},{},{}]}",
                "{blog_id: 7, comments: [{}]}",
                "{blog_id: 3, comments: [{},{},{}]}",
                "{blog_id: 3, comments: [{},{},{},{},{}]}",
                "{blog_id: 2, comments: [{},{},{},{},{},{},{},{}]}",
                "{blog_id: 4, comments: [{},{},{}]}",
                "{blog_id: 5, comments: [{},{}]}",
                "{blog_id: 3, comments: [{},{},{}]}",
                "{blog_id: 5, comments: [{}]}",
            };
            for (int i = 0; i < values.Length; i++)
            {
                db.Documents.Put("docs/" + i, null, RavenJObject.Parse(values[i]), new RavenJObject(), null);
            }

            QueryResult q = null;
            for (var i = 0; i < 5; i++)
            {
                do
                {
                    q = db.Queries.Query("CommentsCountPerBlog", new IndexQuery
                    {
                        Query = "blog_id:3",
                        Start = 0,
                        PageSize = 10
                    }, CancellationToken.None);
                    Thread.Sleep(100);
                } while (q.IsStale);
            }
            q.Results[0].Remove("@metadata");
            Assert.Equal(@"{""blog_id"":3,""comments_length"":14}", q.Results[0].ToString(Formatting.None));
        }

    }
}
