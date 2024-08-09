// -----------------------------------------------------------------------
//  <copyright file="NonHttpBackupRestore.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Smuggler;
using Raven35.Client;
using Raven35.Client.Embedded;
using Raven35.Client.Indexes;
using Raven35.Database.Smuggler;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class NonHttpBackupRestore : RavenTest
    {
        [Fact, Trait("Category", "Smuggler")]
        public async Task CanImportFromDumpFile()
        {
            var file = Path.GetTempFileName();
            using (var store = NewDocumentStoreWithData())
            {
                var dumper = new DatabaseDataDumper(store.SystemDatabase);
                await dumper.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions> { ToFile = file });
            }

            using (var store = NewDocumentStore())
            {
                var dumper = new DatabaseDataDumper(store.SystemDatabase);
                await dumper.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions> { FromFile = file });

                using (var session = store.OpenSession())
                {
                    // Person imported.
                    Assert.Equal(1, session.Query<Person>().Customize(x => x.WaitForNonStaleResults()).Take(5).Count());

                    // Attachment imported.
                    var attachment = store.DatabaseCommands.GetAttachment("Attachments/1");
                    var data = ReadFully(attachment.Data());
                    Assert.Equal(new byte[] { 1, 2, 3 }, data);
                }
            }
        }

        [Fact, Trait("Category", "Smuggler")]
        public async Task ImportReplacesAnExistingDatabase()
        {
            var file = Path.GetTempFileName();

            using (var store = NewDocumentStoreWithData())
            {
                var dumper = new DatabaseDataDumper(store.SystemDatabase);
                await dumper.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions> { ToFile = file });

                using (var session = store.OpenSession())
                {
                    var person = session.Load<Person>(1);
                    person.Name = "Sean Kearon";

                    session.Store(new Person { Name = "Gillian" });

                    store.DatabaseCommands.DeleteAttachment("Attachments/1", null);

                    store.DatabaseCommands.PutAttachment(
                        "Attachments/2",
                        null,
                        new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6 }),
                        new RavenJObject { { "Description", "This is another attachment." } });

                    session.SaveChanges();
                }

                new DatabaseDataDumper(store.SystemDatabase).ImportData(new SmugglerImportOptions<RavenConnectionStringOptions> { FromFile = file }).Wait();
                using (var session = store.OpenSession())
                {
                    // Original attachment has been restored.
                    Assert.NotNull(store.DatabaseCommands.GetAttachment("Attachments/1"));

                    // The newly added attachment is still there.
                    Assert.NotNull(store.DatabaseCommands.GetAttachment("Attachments/2"));

                    // Original person has been restored.
                    Assert.NotNull(session.Query<Person, PeopleByName>().Customize(x => x.WaitForNonStaleResults()).Single(x => x.Name == "Sean"));

                    // The newly added person has not been removed.
                    Assert.True(session.Query<Person, PeopleByName>().Customize(x => x.WaitForNonStaleResults()).Any(x => x.Name == "Gillian"));
                }
            }
        }

        protected override void CreateDefaultIndexes(IDocumentStore documentStore)
        {
            base.CreateDefaultIndexes(documentStore);
            new PeopleByName().Execute(documentStore);
        }

        protected byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private EmbeddableDocumentStore NewDocumentStoreWithData()
        {
            var store = NewDocumentStore();

            using (var session = store.OpenSession())
            {
                session.Store(new Person { Name = "Sean" });
                session.SaveChanges();

                store.DatabaseCommands.PutAttachment(
                    "Attachments/1",
                    null,
                    new MemoryStream(new byte[] { 1, 2, 3 }),
                    new RavenJObject { { "Description", "This is an attachment." } });
            }

            using (var session = store.OpenSession())
            {
                // Ensure the index is built.
                var people = session.Query<Person, PeopleByName>()
                    .Customize(x => x.WaitForNonStaleResults())
                    .Where(x => x.Name == "Sean")
                    .ToArray();
                Assert.NotEmpty(people);
            }

            return store;
        }

        public class PeopleByName : AbstractIndexCreationTask<Person>
        {
            public PeopleByName()
            {
                Map = (persons => from person in persons
                                  select new
                                  {
                                      person.Name,
                                  });
            }
        }

        public class Person
        {
            public string Id;
            public string Name;
        }
    }
}
