using Raven35.Abstractions.Data;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class WildCardQuery : RavenTest
    {
         [Fact]
         public void CanQuery()
         {
             using(var store = NewDocumentStore())
             {
                 store.DatabaseCommands.Query("dynamic", new IndexQuery
                 {
                     Query = "PortalId:0 AND Query:(*) QueryBoosted:(*)"
                 }, new string[0]);
             }
         }
    }
}
