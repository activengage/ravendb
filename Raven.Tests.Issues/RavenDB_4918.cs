using System;
using System.Linq;
using System.Net.Http;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Connection;
using Raven35.Client.Indexes;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_4918 : RavenTestBase
    {
        [Fact]
        public void CanGenerateCSharpDefintionForMultiMap()
        {
            using (var documentStore = NewRemoteDocumentStore())
            {
                documentStore.ExecuteIndex(new MultiMap());

                var request = documentStore.JsonRequestFactory.CreateHttpJsonRequest(
                    new CreateHttpJsonRequestParams(null, documentStore.Url.ForDatabase(documentStore.DefaultDatabase) + "/c-sharp-index-definition/MultiMap", HttpMethod.Get,
                        documentStore.DatabaseCommands.PrimaryCredentials, documentStore.Conventions));

                var response = request.ReadResponseJson().ToString();

                Assert.Contains("from order in docs.Collection1", response);
                Assert.Contains("from order in docs.Collection2", response);
            }
        }


        public class MultiMap : AbstractIndexCreationTask
        {
            public override string IndexName => "MultiMap";

            public override IndexDefinition CreateIndexDefinition()
            {
                return new IndexDefinition
                {
                    Maps =  {
                @"from order in docs.Collection1
select new { order.Company }",
                @"from order in docs.Collection2
select new { order.Company }"
            }
                  
                };
            }
        }

    }
}