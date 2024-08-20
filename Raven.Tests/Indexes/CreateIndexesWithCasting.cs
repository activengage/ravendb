using System.Linq;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Bugs.LiveProjections;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Indexes
{
    public class CreateIndexesWithCasting : NoDisposalNeeded
    {
        [Fact]
        public void WillPreserverTheCasts()
        {
            var indexDefinition = new WithCasting
            {
                Conventions = new DocumentConvention { PrettifyGeneratedLinqExpressions = false}	
            }.CreateIndexDefinition();

            Assert.Contains("docs.People.Select(person => new {", indexDefinition.Map);
            Assert.Contains("Id = ((long) person.Name.Length)", indexDefinition.Map);
        }

        public class WithCasting : AbstractIndexCreationTask<Person>
        {
            public WithCasting()
            {
                Map = persons => from person in persons
                                 select new {Id = (long)person.Name.Length};
            }
        }
    }
}
