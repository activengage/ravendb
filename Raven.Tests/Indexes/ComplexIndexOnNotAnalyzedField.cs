//-----------------------------------------------------------------------
// <copyright file="ComplexIndexOnNotAnalyzedField.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Threading;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Embedded;
using Raven35.Json.Linq;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using Xunit;

namespace Raven35.Tests.Indexes
{
    public class ComplexIndexOnNotAnalyzedField: RavenTest
    {
        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;

        public ComplexIndexOnNotAnalyzedField()
        {
            store = NewDocumentStore();
            db = store.SystemDatabase;
        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanQueryOnKey()
        {
            db.Documents.Put("companies/", null,
                   RavenJObject.Parse("{'Name':'Hibernating Rhinos', 'Partners': ['companies/49', 'companies/50']}"), 
                   RavenJObject.Parse("{'Raven-Entity-Name': 'Companies'}"),
                   null);


            db.Indexes.PutIndex("CompaniesByPartners", new IndexDefinition
            {
                Map = "from company in docs.Companies from partner in company.Partners select new { Partner = partner }",
            });

            QueryResult queryResult;
            do
            {
                queryResult = db.Queries.Query("CompaniesByPartners", new IndexQuery
                {
                    Query = "Partner:companies/49",
                    PageSize = 10
                }, CancellationToken.None);
            } while (queryResult.IsStale);

            Assert.Equal("Hibernating Rhinos", queryResult.Results[0].Value<string>("Name"));
        }
    }
}
