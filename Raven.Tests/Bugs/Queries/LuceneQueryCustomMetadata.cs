using System.Dynamic;
using System.Linq;
using Mono.CSharp;
using Raven35.Abstractions.Linq;
using Raven35.Client.Embedded;
using Raven35.Json.Linq;
using Raven35.Client;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Queries
{
    public class LuceneQueryCustomMetadata : RavenTest
    {
        private const string PropertyName = "MyCustomProperty";

        [Fact]
        public void SuccessTest1()
        {
            using (var documentStore = NewDocumentStore())
            {
                dynamic expando = new ExpandoObject();

                using (var session = documentStore.OpenSession())
                {
                    session.Store(expando);

                    var metadata = session.Advanced.GetMetadataFor((ExpandoObject)expando);

                    metadata[PropertyName] = RavenJToken.FromObject(true);

                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var loaded = session.Load<dynamic>((string)expando.Id);

                    var metadata = session.Advanced.GetMetadataFor((DynamicJsonObject)loaded);

                    var token = metadata[PropertyName];

                    Assert.NotNull(token);
                    Assert.True(token.Value<bool>());
                }
            }
        }

        [Fact]
        public void SuccessTest2()
        {
            using (var documentStore = NewDocumentStore())
            {
                dynamic expando = new ExpandoObject();

                using (var session = documentStore.OpenSession())
                {
                    session.Store(expando);

                    RavenJObject metadata =
                        session.Advanced.GetMetadataFor((ExpandoObject)expando);

                    metadata[PropertyName] = RavenJToken.FromObject(true);

                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    dynamic loaded = session.Advanced.DocumentQuery<dynamic>()
                        .WhereEquals("@metadata.Raven-Entity-Name",
                                     documentStore.Conventions.GetTypeTagName(typeof(ExpandoObject)))
                        .FirstOrDefault();

                    Assert.NotNull(loaded);
                }
            }
        }

        [Fact]
        public void FailureTest()
        {
            using (var documentStore = NewDocumentStore())
            {
                dynamic expando = new ExpandoObject();

                using (var session = documentStore.OpenSession())
                {
                    session.Store(expando);

                    RavenJObject metadata =
                        session.Advanced.GetMetadataFor((ExpandoObject)expando);

                    metadata[PropertyName] = RavenJToken.FromObject(true);

                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    dynamic loaded =
                        session.Advanced.DocumentQuery<dynamic>().WhereEquals("@metadata." + PropertyName, true)
                            .FirstOrDefault();

                    Assert.NotNull(loaded);
                }
            }
        }
    }
}
