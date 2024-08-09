using System.Linq;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class IndexWithUnion : RavenTest
    {
        public class Index : AbstractMultiMapIndexCreationTask
        {
            public Index()
            {
                AddMap<Ronne.Sermon>(items => from x in items
                                              select new
                                              {
                                                Content =
                                                new string[]
                                                {
                                                    x.Description, x.Series, x.Speaker,
                                                    x.Title
                                                }.Union(x.Tags)
                                              });
            }
        }

         [Fact]
        public void CanCreateIndex()
         {
             using(var x = NewDocumentStore())
             {
                new Index().Execute(x);
             }
            
         }
    }
}
