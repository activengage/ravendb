using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3864 : RavenTest
    {
        private readonly DocumentConvention conventions = new DocumentConvention
        {
            PrettifyGeneratedLinqExpressions = false
        };

        [Fact]
        public void can_use_conventions_with_create_indexes_container()
        {
            using (var store = NewDocumentStore())
            {
                var container = new CompositionContainer(new TypeCatalog(typeof (CustomIdInIndexCreationTask)));
                IndexCreation.CreateIndexes(container, store.DatabaseCommands, conventions);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public async Task can_use_conventions_with_create_indexes_async_container()
        {
            using (var store = NewDocumentStore())
            {
                var container = new CompositionContainer(new TypeCatalog(typeof(CustomIdInIndexCreationTask)));
                await IndexCreation.CreateIndexesAsync(container, store.AsyncDatabaseCommands, conventions);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public void can_use_conventions_with_create_side_by_side_indexes_container()
        {
            using (var store = NewDocumentStore())
            {
                var container = new CompositionContainer(new TypeCatalog(typeof(CustomIdInIndexCreationTask)));
                IndexCreation.SideBySideCreateIndexes(container, store.DatabaseCommands, conventions);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public async Task can_use_conventions_with_create_side_by_side_indexes_async_container()
        {
            using (var store = NewDocumentStore())
            {
                var container = new CompositionContainer(new TypeCatalog(typeof(CustomIdInIndexCreationTask)));
                await IndexCreation.SideBySideCreateIndexesAsync(container, store.AsyncDatabaseCommands, conventions);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public void can_use_conventions_with_create_indexes()
        {
            using (var store = NewDocumentStore(conventions: conventions))
            {
                var list = new List<AbstractIndexCreationTask>
                {
                    new CustomIdInIndexCreationTask(),
                    new CustomIdWithNameInIndexCreationTask()
                };

                store.ExecuteIndexes(list);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public void can_use_conventions_with_create_indexes_async()
        {
            using (var store = NewDocumentStore(conventions: conventions))
            {
                var list = new List<AbstractIndexCreationTask>
                {
                    new CustomIdInIndexCreationTask(),
                    new CustomIdWithNameInIndexCreationTask()
                };

                store.ExecuteIndexesAsync(list);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public void can_use_conventions_with_create_side_by_side_indexes()
        {
            using (var store = NewDocumentStore(conventions: conventions))
            {
                var list = new List<AbstractIndexCreationTask>
                {
                    new CustomIdInIndexCreationTask(),
                    new CustomIdWithNameInIndexCreationTask()
                };

                store.ExecuteIndexes(list);
                Assert.True(testFailed == false);
            }
        }

        [Fact]
        public void can_use_conventions_with_create_side_by_side_indexes_async()
        {
            using (var store = NewDocumentStore(conventions: conventions))
            {
                var list = new List<AbstractIndexCreationTask>
                {
                    new CustomIdInIndexCreationTask(),
                    new CustomIdWithNameInIndexCreationTask()
                };

                store.ExecuteIndexesAsync(list);
                Assert.True(testFailed == false);
            }
        }

        private static bool testFailed = false;
        [Export(typeof(AbstractIndexCreationTask<Data>))]
        public class CustomIdInIndexCreationTask: AbstractIndexCreationTask<Data> 
        {
            public CustomIdInIndexCreationTask()
            {
                Map = docs => from doc in docs select new { doc.CustomId };				
            }

            public override IndexDefinition CreateIndexDefinition()
            {
                if (Conventions == null || Conventions.PrettifyGeneratedLinqExpressions == true) testFailed = true;
                return base.CreateIndexDefinition();
            }
        }

        [Export(typeof(AbstractIndexCreationTask<Data>))]
        public class CustomIdWithNameInIndexCreationTask : AbstractIndexCreationTask<Data>
        {
            public CustomIdWithNameInIndexCreationTask()
            {
                Map = docs => from doc in docs select new
                {
                    doc.CustomId,
                    doc.Name
                };
            }

            public override IndexDefinition CreateIndexDefinition()
            {
                if (Conventions == null || Conventions.PrettifyGeneratedLinqExpressions == true) testFailed = true;
                return base.CreateIndexDefinition();
            }
        }

        public class Data
        {
            public string CustomId { get; set; }
            public string Name { get; set; }
        }
    }
}
