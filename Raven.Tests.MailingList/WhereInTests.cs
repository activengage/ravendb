using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class WhereInTests : RavenTest
    {
        [Fact]
        public void WhereIn_using_index_notAnalyzed()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {
                new PersonsNotAnalyzed().Execute(documentStore);

                string[] names = { "Person One", "PersonTwo" };

                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Advanced.DocumentQuery<Person, PersonsNotAnalyzed>().WhereIn(p => p.Name, names);
                    Assert.Equal(2, query.ToList().Count());
                }
            }
        }

        [Fact]
        public void SameHash()
        {
            var perFieldAnalyzerComparer = new RavenPerFieldAnalyzerWrapper.PerFieldAnalyzerComparer();
            Assert.Equal(perFieldAnalyzerComparer.GetHashCode("Name"), perFieldAnalyzerComparer.GetHashCode("@in<Name>"));
            Assert.True(perFieldAnalyzerComparer.Equals("Name", "@in<Name>"));
        }

        [Fact]
        public void WhereIn_using_index_analyzed()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {
                new PersonsAnalyzed().Execute(documentStore);

                string[] names = { "Person One", "PersonTwo" };

                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Advanced.DocumentQuery<Person, PersonsAnalyzed>().WhereIn(p => p.Name, names);
                    Assert.Equal(2, query.ToList().Count());
                }
            }
        }

        [Fact]
        public void WhereIn_not_using_index()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {

                string[] names = { "Person One", "PersonTwo" };

                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Advanced.DocumentQuery<Person>().WhereIn(p => p.Name, names);
                    Assert.Equal(2, query.ToList().Count());
                }
            }
        }

        [Fact]
        public void Where_In_Values_With_Space_And_AND_Operator()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {
                string[] names = { "Person One", "PersonTwo" };
                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Advanced.DocumentQuery<Person>().UsingDefaultOperator(QueryOperator.And).WhereIn(p => p.Name, names);
                    Assert.Equal(2, query.ToList().Count());
                }
            }

        }

        [Fact]
        public void Where_In_using_query_index_notAnalyzed()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {
                new PersonsNotAnalyzed().Execute(documentStore);

                string[] names = { "Person One", "PersonTwo" };

                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Query<Person, PersonsNotAnalyzed>().Where(p => p.Name.In(names));
                    Assert.Equal(2, query.Count());
                }
            }
        }

        [Fact]
        public void Where_In_using_query_index_analyzed()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {
                new PersonsAnalyzed().Execute(documentStore);

                string[] names = { "Person One", "PersonTwo" };

                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Query<Person, PersonsAnalyzed>().Where(p => p.Name.In(names));
                    Assert.Equal(2, query.Count());
                }
            }
        }

        [Fact]
        public void Where_In_using_query()
        {
            using (IDocumentStore documentStore = NewDocumentStore())
            {
                string[] names = { "Person One", "PersonTwo" };

                StoreObjects(new List<Person>
                {
                    new Person {Name = names[0]},
                    new Person {Name = names[1]}
                }, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var query = session.Query<Person>().Where(p => p.Name.In(names));
                    Assert.Equal(2, query.Count());
                }
            }
        }

        private void StoreObjects<T>(IEnumerable<T> objects, IDocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                foreach (var o in objects)
                {
                    session.Store(o);
                }
                session.SaveChanges();
            }
            WaitForIndexing(documentStore);
        }
    }

    public class Person
    {
        public string Name { get; set; }
    }

    public class PersonsNotAnalyzed : AbstractIndexCreationTask<Person>
    {
        public PersonsNotAnalyzed()
        {
            Map = organizations => from o in organizations
                                   select new { o.Name };

            Indexes.Add(x => x.Name, FieldIndexing.NotAnalyzed);
        }
    }

    public class PersonsAnalyzed : AbstractIndexCreationTask<Person>
    {
        public PersonsAnalyzed()
        {
            Map = organizations => from o in organizations
                                   select new { o.Name };

            Indexes.Add(x => x.Name, FieldIndexing.Analyzed);
        }
    }
}
