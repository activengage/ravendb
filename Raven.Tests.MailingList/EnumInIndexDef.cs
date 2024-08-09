using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class EnumInIndexDef : RavenTest
    {
        [Fact]
        public void QueryById()
        {
            using (var store = NewDocumentStore())
            {
                new SomeDocumentIndex().Execute(store);
            }
        }

        public class SomeDocument
        {
            public string Id { get; set; }
            public string Text { get; set; }
        }

        public enum SomeEnum
        {
            Value1 = 1,
            Value2 = 2
        }

        public class SomeDocumentIndex : AbstractIndexCreationTask<SomeDocument, SomeDocumentIndex.IndexResult>
        {
            public class IndexResult
            {
                public string Id { get; set; }
                public SomeEnum SomeEnum { get; set; }
            }

            public SomeDocumentIndex()
            {
                Map = docs => from doc in docs
                              select new { Id = doc.Id, SomeEnum = SomeEnum.Value1 };

                Store(x => x.SomeEnum, FieldStorage.Yes);
            }
        }
    }
}
