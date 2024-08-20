using System;
using Raven35.Tests.Common;
using Xunit;
using Raven35.Client;
using Raven35.Abstractions.Indexing;
using Raven35.Json.Linq;

namespace Raven35.Tests.ResultsTransformer
{
    public class NullDynamicValuesInTransformerOperation : RavenTest
    {
        public class Person
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public void TransformerOperationWithNullDynamicValues()
        {
            using (var store = NewDocumentStore())
            {
                PerformTestWithNullDynamicValues(store);
            }
        }

        private void PerformTestWithNullDynamicValues(IDocumentStore store)
        {
            // 1. Create document
            using (var session = store.OpenSession())
            {
                session.Store(new Person { Id = "persons/1" });
                session.SaveChanges();
            }

            string[] operations = new string[] { "+", "Plus", "-", "Minus", "%", "Percent", "*", "Mult", "/", "Divide" };
            for (int i = 0; i < operations.Length - 1; i += 2)
            {
                // 2. Define transformer with an operation on a field that will be null at run time
                string transformerName = $"NullDynamicValueTransformer{operations[i+1]}Operation";
                string transformer = $"from u in results select new {{ Age = u.Age {operations[i]} 1 }}";

                store.DatabaseCommands.PutTransformer(transformerName, new TransformerDefinition
                {
                    Name = transformerName,
                    TransformResults = transformer
                });

                // 3. Retrieve Document with transformer
                var document = store.DatabaseCommands.Get(new[] { "persons/1" }, null, transformerName);

                // 4. 'Age' should be null, since it is not part of Person document
                var valueOfAge = document.Results[0]["$values"].Value<RavenJArray>()[0].Value<RavenJObject>()["Age"].Value<object>();
                Assert.Null(valueOfAge);
            }
        }
    }
}