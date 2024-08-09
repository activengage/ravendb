using System;
using System.Linq;
using System.Threading.Tasks;
using Raven35.Client;
using Raven35.Client.Linq;
using Raven35.Tests.Common.Dto;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDb_4706 : RavenTestBase
    {
        [Fact]
        public async Task SupportRandomOrder()
        {
            using (var documentStore = NewDocumentStore())
            {
                using (var session = documentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(new User {Name = "Fitzchak Yitzchaki"});
                    await session.StoreAsync(new User {Name = "Oren Eini"});
                    await session.StoreAsync(new User {Name = "Maxim Buryak" });
                    await session.StoreAsync(new User {Name = "Grisha Kotler" });
                    await session.StoreAsync(new User {Name = "Michael Yarichuk" });
                    await session.SaveChangesAsync();
                }

                using (var session = documentStore.OpenAsyncSession())
                {
                    var users = await session.Query<User>()
                        .Customize(customization => customization.RandomOrdering())
                        .Where(product => product.Name != "Fitzchak Yitzchaki")
                        .Take(2)
                        .ToListAsync();

                    Assert.Equal(2, users.Count);
                }
            }
        }
    }
}