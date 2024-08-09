using System.Linq;
using Raven35.Client;
using Raven35.Client.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class AllPropertiesIndex : RavenTest
    {
        public class Users_AllProperties : AbstractIndexCreationTask<User, Users_AllProperties.Result>
        {
            public class Result
            {
                public string Query { get; set; }
            }
            public Users_AllProperties()
            {
                Map = users =>
                      from user in users
                      select new
                      {
                          Query = AsDocument(user).Select(x => x.Value)
                      };
                Index(x=>x.Query, FieldIndexing.Analyzed);
            }
        }

        [Fact]
        public void CanSearchOnAllProperties()
        {
            using(var store = NewDocumentStore())
            {
                new Users_AllProperties().Execute(store);

                using(var s = store.OpenSession())
                {
                    s.Store(new User
                    {
                        FirstName = "Ayende",
                        LastName = "Rahien"
                    });

                    s.SaveChanges();
                }

                using (var s = store.OpenSession())
                {
                    Assert.NotEmpty(s.Query<Users_AllProperties.Result, Users_AllProperties>()
                                        .Customize(x=>x.WaitForNonStaleResults())
                                        .Where(x=>x.Query == "Ayende")
                                        .As<User>()
                                        .ToList());

                    Assert.NotEmpty(s.Query<Users_AllProperties.Result, Users_AllProperties>()
                                        .Customize(x => x.WaitForNonStaleResults())
                                        .Where(x => x.Query == "Ayende")
                                        .As<User>()
                                        .ToList());

                }
            }
            
        }
         
    }
}
