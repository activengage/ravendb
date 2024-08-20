using Raven35.Client.Document;
using Raven35.Client.Extensions;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.MultiTenancy
{
    public class NoCaseSensitive : RavenTest
    {
        [Fact]
        public void CanAccessDbUsingDifferentNames()
        {
            using (GetNewServer())
            {
                using (var documentStore = new DocumentStore
                {
                    Url = "http://localhost:8079"
                })
                {
                    documentStore.Initialize();
                    documentStore.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("repro");
                    using (var session = documentStore.OpenSession("repro"))
                    {
                        session.Store(new Foo
                        {
                            Bar = "test"
                        });
                        session.SaveChanges();
                    }

                    using (var session = documentStore.OpenSession("Repro"))
                    {
                        Assert.NotNull(session.Load<Foo>("foos/1"));
                    }
                }
            }
        }


        internal class Foo
        {
            public string Bar { get; set; }
        }
    }
}
