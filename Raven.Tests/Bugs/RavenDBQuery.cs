using System;
using System.Linq;
using Raven35.Client;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto.TagCloud;
using Raven35.Tests.Document;
using Xunit;

namespace Raven35.Tests.Bugs
{
    public class RavenDBQuery : RavenTest
    {
        [Fact]
        public void CanPerformQueryOnParameter()
        {
            using(var store = NewDocumentStore())
            {
                Tag("test", store.OpenSession());
            }
        }

        public void Tag(string slug, IDocumentSession session)
        {

            var postsQuery = from post in session.Query<Post>()
                             where post.Tags.Any(postTag => postTag == slug)
                             select post;

            postsQuery.ToList();
        }
    }
}
