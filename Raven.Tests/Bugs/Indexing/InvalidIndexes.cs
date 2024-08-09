using System;
using System.Linq;
using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class InvalidIndexes : RavenTest
    {
        [Fact]
        public void CannotCreateIndexesUsingDateTimeNow()
        {
            using (var store = NewDocumentStore())
            {
                var ioe = Assert.Throws<IndexCompilationException>(() =>
                                                                   store.DatabaseCommands.PutIndex("test",
                                                                                                   new IndexDefinition
                                                                                                   {
                                                                                                    Map =
                                                                                                        @"from user in docs.Users 
where user.LastLogin > DateTime.Now.AddDays(-10) 
select new { user.Name}"
                                                                                                   }));

                Assert.Contains(@"Cannot use DateTime.Now during a map or reduce phase.", ioe.Message);
            }
        }

        [Fact]
        public void CannotCreateIndexWithOrderBy()
        {
            using(var store = NewDocumentStore())
            {
                var ioe = Assert.Throws<IndexCompilationException>(() =>
                                                                                         store.DatabaseCommands.PutIndex("test",
                                                                                                                         new IndexDefinition
                                                                                                                         {
                                                                                                                            Map =
                                                                                                                                "from user in docs.Users orderby user.Id select new { user.Name}"
                                                                                                                         }));

                Assert.Contains(@"OrderBy calls are not valid during map or reduce phase, but the following was found:", ioe.Message);
            }
        }
    }
}
