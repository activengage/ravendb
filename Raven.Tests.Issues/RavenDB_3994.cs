using System.Linq;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Json.Linq;
using Raven35.Tests.Bundles.ScriptedIndexResults;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3994 : RavenTest
    {
        protected override void ModifyConfiguration(Database.Config.InMemoryRavenConfiguration configuration)
        {
            configuration.Settings["Raven/ActiveBundles"] = "ScriptedIndexResults";
        }

        [Fact]
        public void EachDocumentOutputHasItsOwnKey()
        {
            using (var store = NewDocumentStore())
            {
                using (var s = store.OpenSession())
                {
                    s.Store(new ScriptedIndexResults
                    {
                        Id = ScriptedIndexResults.IdPrefix + new AnimalsPseudoReduce().IndexName,
                        IndexScript = @"",
                        DeleteScript = @"PutDocument('DeleteScriptRan', {})"
                    });
                    s.SaveChanges();
                }
                string docId;
                using (var s = store.OpenSession())
                {
                    var animal = new Animal
                    {
                        Id = "pluto",
                        Name = "Pluto",
                        Type = "Dog"
                    };
                    s.Store(animal);

                    docId = s.Advanced.GetDocumentId(animal);

                    s.SaveChanges();
                }

                new AnimalsPseudoReduce().Execute(store);

                WaitForIndexing(store);
                
                store.DatabaseCommands.Delete(docId, null);

                WaitForIndexing(store);

                using (var s = store.OpenSession())
                {
                    Assert.NotNull(s.Load<RavenJObject>("DeleteScriptRan"));
                }
            }
        }


        public class AnimalsPseudoReduce : AbstractMultiMapIndexCreationTask<AnimalsPseudoReduce.Result>
        {
            public class Result
            {
                public string Id { get; set; }
                public string Name { get; set; }
            }
            public AnimalsPseudoReduce()
            {
                AddMap<Animal>(animals =>
                    from animal in animals
                    select new
                    {
                        animal.Id,
                        animal.Name
                    });

                Reduce = animals => from animal in animals
                    group animal by animal.Id into a
                    select new
                    {
                        Id = a.Single().Id,
                        Name = a.Single().Name
                    };

                Store(a => a.Name, FieldStorage.Yes);
            }
        }
    }
}