using System;
using System.Linq;
using Raven35.Client.Embedded;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class NullableGuidIndexTest : RavenTest
    {
        public class TestDocument
        {
            public string Id { get; set; }

            public Guid? OptionalExternalId { get; set; }
        }

        public class TestDocumentIndex : AbstractIndexCreationTask<TestDocument>
        {
            public TestDocumentIndex()
            {
                Map = docs => from doc in docs
                              where doc.OptionalExternalId != null
                              select new { doc.OptionalExternalId };
            }
        }

        [Fact]
        public void Can_query_against_nullable_guid()
        {
            using (var store = NewDocumentStore())
            {
                new TestDocumentIndex().Execute(store.DatabaseCommands, store.Conventions);

                using (var session = store.OpenSession())
                {
                    session.Store(new TestDocument());
                    session.Store(new TestDocument { OptionalExternalId = Guid.NewGuid() });
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    TestDocument[] results = session.Query<TestDocument, TestDocumentIndex>()
                        .Customize(c => c.WaitForNonStaleResultsAsOfLastWrite())
                        .ToArray();
                    Assert.Empty(store.SystemDatabase.Statistics.Errors);
                    Assert.NotEmpty(results);
                }
            }
        }
    }
}
