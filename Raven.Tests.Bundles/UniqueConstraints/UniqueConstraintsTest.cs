using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;

using Raven35.Bundles.UniqueConstraints;
using Raven35.Client.Embedded;
using Raven35.Client.UniqueConstraints;
using Raven35.Tests.Common;

namespace Raven35.Tests.Bundles.UniqueConstraints
{
    public abstract class UniqueConstraintsTest : RavenTest
    {
        protected UniqueConstraintsTest()
        {
            DocumentStore = NewDocumentStore(port: 8079, configureStore: store =>
            {                
                store.Configuration.Catalog.Catalogs.Add(new AssemblyCatalog(typeof(UniqueConstraintsPutTrigger).Assembly));
                store.RegisterListener(new UniqueConstraintsStoreListener());
            },activeBundles: "Unique Constraints");
        }

        protected EmbeddableDocumentStore DocumentStore { get; set; }
    }

    public class User
    {
        public string Id { get; set; }

        [UniqueConstraint]
        public string Email { get; set; }

        public string Name { get; set; }

        [UniqueConstraint]
        public string[] TaskIds { get; set; }
    }

    public class Foo
    {
        [UniqueConstraint]
        public List<string> UniqueStrings { get; set; } 
    }

    public class GenericNamedValue<T>
    {
        public string Id { get; set; }

        [UniqueConstraint]
        public string Name { get; set; }

        public T Value { get; set; }
    }
}
