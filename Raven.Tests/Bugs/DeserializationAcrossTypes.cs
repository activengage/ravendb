using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    namespace First
    {
        public class Alpha
        {
            public string Foo { get; set; }
        }
    }

    public class DeserializationAcrossTypes : RavenTest
    {
        [Fact]
        public void can_deserialize_across_types_when_origin_type_doesnt_exist()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.Put("alphas/1", null, RavenJObject.Parse("{ 'Foo': 'Bar'}"),
                                           RavenJObject.Parse(
                                               "{'Raven-Clr-Type': 'Raven35.Tests.Bugs.Second.Alpha', 'Raven-Entity-Name': 'Alphas' }"));

                using (var session = store.OpenSession())
                {
                    session.Load<First.Alpha>("alphas/1");
                }
            }
        }
    }
}
