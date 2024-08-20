using Raven35.Client.Embedded;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Embedded
{
    public class CanUseForUrlOnly : NoDisposalNeeded
    {
        [Fact]
        public void WontCreateDirectory()
        {
            using (var embeddableDocumentStore = new EmbeddableDocumentStore
            {
                Url = "http://localhost:8079"
            })
            {
                embeddableDocumentStore.Initialize();
                Assert.Null(embeddableDocumentStore.SystemDatabase);
            }
        }

        [Fact]
        public void WontCreateDirectoryWhenSettingStorage()
        {
            using (var embeddableDocumentStore = new EmbeddableDocumentStore
            {
                Configuration =
                {
                    DefaultStorageTypeName = "voron"
                },
                Url = "http://localhost:8079"
            })
            {
                embeddableDocumentStore.Initialize();
                Assert.Null(embeddableDocumentStore.SystemDatabase);
            }
        }
    }
}
