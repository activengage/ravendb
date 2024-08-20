using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class Companies_AllProperties : AbstractIndexCreationTask<Company, Companies_AllProperties.Result>
    {
        public class Result
        {
            public string Query { get; set; }
        }

        public Companies_AllProperties()
        {
            Map = companies => from company in companies
                               select new
                               {
                                   Query = AsDocument(company).Select(x => x.Value)
                               };

            Index(x => x.Query, FieldIndexing.Analyzed);
        }
    }
}
