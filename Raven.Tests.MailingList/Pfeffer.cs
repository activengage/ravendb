using System.Collections.Generic;
using System.Linq;
using Raven35.Client.Embedded;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Pfeffer : RavenTest
    {
        [Fact]
        public void QueryingUsingObjects()
        {
            using (var store =  NewDocumentStore())
            {
                store.Conventions.CustomizeJsonSerializer += serializer =>
                {
                    serializer.TypeNameHandling = TypeNameHandling.All;
                };
                using (var session = store.OpenSession())
                {
                    var obj = new Outer { Examples = new List<IExample>() { new Example { Provider = "Test", Id = "Abc" } } };
                    session.Store(obj);
                    session.SaveChanges();
                }
                using (var session = store.OpenSession())
                {
                    var ex = new Example { Provider = "Test", Id = "Abc" };
                    var arr = session.Query<Outer>().Customize(c => c.WaitForNonStaleResults())
                        .Where(o => o.Examples.Any(e => e == ex))
                        .ToArray();
                    WaitForUserToContinueTheTest(store);
                    Assert.Equal(1, arr.Length);
                }
            }
        }

        // Define other methods and classes here
        public interface IExample
        {
        }

        public class Outer
        {
            public int Id { get; set; }
            public IList<IExample> Examples { get; set; }
        }

        public class Example : IExample
        {
            public string Provider { get; set; }
            public string Id { get; set; }
        }
    }
}
