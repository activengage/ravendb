using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;

namespace Raven35.Tests.Bugs.Indexing
{
    public class CanIndexWithCharLiteral : RavenTest
    {
        [Fact]
        public void CanQueryDocumentsIndexWithCharLiteral()
        {
            using (var store = NewDocumentStore()) {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition {
                    Map = "from doc in docs select  new { SortVersion = doc.Version.PadLeft(5, '0') }",
                    Stores = new[] { new { Field = "SortVersion", Storage = FieldStorage.Yes } }.ToDictionary(d => d.Field, d => d.Storage)
                });

                using (var s = store.OpenSession()) {
                    var entity = new { Version = "1" };
                    s.Store(entity);
                    s.SaveChanges();
                }

                using (var s = store.OpenSession()) {
                    Assert.Equal(1, s.Query<object>("test").Customize(x => x.WaitForNonStaleResults()).Count());
                    Assert.Equal("00001", s.Query<object>("test").Customize(x => x.WaitForNonStaleResults()).ProjectFromIndexFieldsInto<Result>().First().SortVersion);
                }
            }
        }

        private class Result
        {
            public string SortVersion { get; set; }
        }
    }
}
