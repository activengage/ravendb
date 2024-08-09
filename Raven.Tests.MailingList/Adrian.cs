using System.Collections.Generic;
using System.Linq;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Adrian : RavenTest
    {
        public class ContentDescriptorByMetadata : AbstractIndexCreationTask<ContentDescriptor>
        {
            public ContentDescriptorByMetadata()
            {
                Map = cds =>
                    from cd in cds
                    select new
                    {
                        _ = cd.Identify.Select(x => CreateField(x.Key, x.Value))
                    };
            }
        }

        public class ContentDescriptor
        {
            public Dictionary<string, string> Identify { get; set; }
        }

        [Fact]
        public void CanCreateIndex()
        {
            using(var store = NewDocumentStore())
            {
                new ContentDescriptorByMetadata().Execute(store);
            }
        }

        [Fact]
        public void CanCreateIndex_Remote()
        {
            using(GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                new ContentDescriptorByMetadata().Execute(store);
            }
        }
    }
}
