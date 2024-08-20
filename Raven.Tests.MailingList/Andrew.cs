using System.Linq;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Andrew : RavenTest
    {
        [Fact]
        public void CanCompile()
        {
            var technologySummaryIndex = new TechnologySummary_Index {Conventions = new DocumentConvention
            {
                PrettifyGeneratedLinqExpressions = false
            }};

            var indexDefinition = technologySummaryIndex.CreateIndexDefinition();

            Assert.Equal(
                @"docs.Technologies.Where(technology => !technology.__document_id.EndsWith(""/published"")).Select(technology => new {
    TechnologyId = technology.__document_id,
    DrugId = technology.Drug.Id
})",
                indexDefinition.Map);
        }

        public class TechnologySummary_Index : AbstractIndexCreationTask<Technology, TechnologySummary>
        {
            public TechnologySummary_Index()
            {
                Map = (technologies => from technology in technologies
                                       where !technology.Id.EndsWith("/published")
                                       select new
                                       {
                                           TechnologyId = technology.Id,
                                           DrugId = technology.Drug.Id,
                                       });

                Reduce = results => from result in results
                                    group result by result.TechnologyId
                                        into g
                                        let rec = g.LastOrDefault()
                                        select
                                            new
                                            {
                                                rec.TechnologyId,
                                                rec.DrugId,
                                            };
            }
        }

        public class TechnologySummary
        {
            public string TechnologyId;
            public string DrugId;
        }

        public class Technology
        {
            public string Id;
            public Drug Drug;
        }

        public class Drug
        {
            public string Id;
        }
    }
}
