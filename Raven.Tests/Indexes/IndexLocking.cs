using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Indexes
{
    public class IndexLocking : RavenTest
    {
        [Fact]
        public void LockingIndexesInMemoryWillNotFail()
        {
            using (var store = NewDocumentStore())
            {
                var index = new IndexSample
                {
                    Conventions = new DocumentConvention()
                };
                index.Execute(store);

                var indexDefinition = store.SystemDatabase.IndexDefinitionStorage.GetIndexDefinition("IndexSample");

                indexDefinition.LockMode = IndexLockMode.LockedIgnore;
                store.SystemDatabase.IndexDefinitionStorage.UpdateIndexDefinitionWithoutUpdatingCompiledIndex(indexDefinition);

                indexDefinition = store.SystemDatabase.IndexDefinitionStorage.GetIndexDefinition("IndexSample");
                Assert.Equal(indexDefinition.LockMode, IndexLockMode.LockedIgnore);
            }
        }

        public class IndexSample : AbstractIndexCreationTask<Contact>
        {
            public IndexSample()
            {
                Map = contacts =>
                    from contact in contacts
                    select new
                    {
                        contact.FirstName,
                        PrimaryEmail_EmailAddress = contact.PrimaryEmail.Email,
                    };
            }
        }

        public class Contact
        {
            public string FirstName { get; set; }
            public EmailAddress PrimaryEmail { get; set; }
        }

        public class EmailAddress
        {
            public string Email { get; set; }
        }
    }
}
