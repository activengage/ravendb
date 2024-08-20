using Raven35.Client.Document;
using Raven35.Client.Extensions;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class Issue355 : RavenTest
    {
        private string _ravenUrl = "http://localhost:8079";
        private string _ravenDatabaseName = "Northwind";

        [Fact]
        public void ShouldNotThrow()
        {
            using(GetNewServer())
            {
                EnsureDatabaseExists();	
            }	
        }

        public DocumentStore GetDocumentStore()
        {
            var doc = new DocumentStore();
            doc.Url = _ravenUrl;
            doc.Conventions = new DocumentConvention()
            {
                FindIdentityProperty = p => p.Name.Equals("RavenDocumentId")
            };
            return doc;
        }

        public void EnsureDatabaseExists()
        {
            using (var store = GetDocumentStore())
            {
                store.Initialize();
                store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(_ravenDatabaseName);
            }
        }

    }
}
