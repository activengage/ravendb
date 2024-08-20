using Raven35.Imports.Newtonsoft.Json;
using Raven35.Client.Embedded;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class SerializingAndDeserializingWithRaven : RavenTest
    {
        [Fact]
        public void can_deserialize_id_with_private_setter()
        {
            using (var documentStore = NewDocumentStore())
            using (var session = documentStore.OpenSession())
            {
                var testObj = new TestObj(1000, 123);
                session.Store(testObj);
                session.SaveChanges();
                session.Advanced.Clear();
                var load = session.Load<TestObj>(1000);

                Assert.Equal(123, load.AnotherLong);
                Assert.Equal(1000, load.Id);
            }
        }

        public abstract class AggregateRoot
        {
            protected AggregateRoot()
            {
            }

            protected AggregateRoot(long id, long anotherLong)
            {
                Id = id;
                AnotherLong = anotherLong;
            }

            public long Id { get; private set; }
            public long AnotherLong { get; private set; }
        }

        public class TestObj : AggregateRoot
        {
            [JsonConstructor]
            public TestObj()
            {
            }

            public TestObj(long id, long anotherLong)
                : base(id,
                    anotherLong)
            {
            }
        }
    }
}
