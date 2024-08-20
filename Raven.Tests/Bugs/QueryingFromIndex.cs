//-----------------------------------------------------------------------
// <copyright file="QueryingFromIndex.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Raven35.Tests.Document;
using Xunit;

namespace Raven35.Tests.Bugs
{
    public class QueryingFromIndex : RavenTest
    {
        [Fact]
        public void LuceneQueryWithIndexIsCaseInsensitive()
        {
            using (var store = this.NewDocumentStore())
            {
                var definition = new IndexDefinitionBuilder<Company>
                {
                    Map = docs => from doc in docs
                                  select new
                                  {
                                      doc.Name
                                  }
                }.ToIndexDefinition(store.Conventions);
                store.DatabaseCommands.PutIndex("CompanyByName",
                                                definition);

                using (var session = store.OpenSession())
                {
                    session.Store(new Company { Name = "Google" });
                    session.Store(new Company
                    {
                        Name =
                            "HibernatingRhinos"
                    });
                    session.SaveChanges();

                    var company =
                        session.Advanced.DocumentQuery<Company>("CompanyByName")
                            .Where("Name:Google")
                            .WaitForNonStaleResults()
                            .FirstOrDefault();

                    Assert.NotNull(company);
                }
            }
        }

        [Fact]
        public void LinqQueryWithIndexIsCaseInsensitive()
        {
            using (var store = this.NewDocumentStore())
            {
                var definition = new IndexDefinitionBuilder<Company>
                {
                    Map = docs => from doc in docs
                                  select new
                                  {
                                      doc.Name
                                  }
                }.ToIndexDefinition(store.Conventions);
                store.DatabaseCommands.PutIndex("CompanyByName",
                                                definition);

                using (var session = store.OpenSession())
                {
                    session.Store(new Company { Name = "Google" });
                    session.Store(new Company
                    {
                        Name =
                            "HibernatingRhinos"
                    });
                    session.SaveChanges();

                    var company =
                        session.Query<Company>("CompanyByName")
                            .Customize(x=>x.WaitForNonStaleResults())
                            .Where(x=>x.Name == "Google")
                            .FirstOrDefault();

                    Assert.NotNull(company);
                }
            }
        }
    }
}
