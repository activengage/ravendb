using System;
using System.IO;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Smuggler;
using Raven35.Smuggler;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class SmugglerTests : RavenTest
    {
        public class Foo
        {
            public string Id { get; set; }
            public DateTime Created { get; set; }
        }

        [Fact, Trait("Category", "Smuggler")]
        public void DateTimePreserved()
        {
            var file = Path.GetTempFileName();

            try
            {
                var docId = string.Empty;

                using (var documentStore = NewRemoteDocumentStore())
                {
                    using (var session = documentStore.OpenSession())
                    {
                        var foo = new Foo {Created = DateTime.Today};
                        session.Store(foo);
                        docId = foo.Id;
                        session.SaveChanges();
                    }
                    var smugglerApi = new SmugglerDatabaseApi();
                    smugglerApi.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions> { ToFile = file, From = new RavenConnectionStringOptions { Url = documentStore.Url, DefaultDatabase = documentStore.DefaultDatabase } }).Wait(TimeSpan.FromSeconds(15));
                }

                using (var documentStore = NewRemoteDocumentStore())
                {
                    var smugglerApi = new SmugglerDatabaseApi();
                    smugglerApi.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions> { FromFile = file, To = new RavenConnectionStringOptions { Url = documentStore.Url, DefaultDatabase = documentStore.DefaultDatabase } }).Wait(TimeSpan.FromSeconds(15));
                    
                    using (var session = documentStore.OpenSession())
                    {
                        var created = session.Load<Foo>(docId).Created;
                        Assert.False(session.Advanced.HasChanges);
                    }
                }
            }
            finally
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
