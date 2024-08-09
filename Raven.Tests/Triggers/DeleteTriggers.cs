//-----------------------------------------------------------------------
// <copyright file="DeleteTriggers.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition.Hosting;
using Raven35.Client.Embedded;
using Raven35.Json.Linq;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Tests.Common;
using Raven35.Tests.Storage;
using Xunit;

namespace Raven35.Tests.Triggers
{
    public class DeleteTriggers : RavenTest
    {
        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;

        public DeleteTriggers()
        {
            store = NewDocumentStore( catalog: (new TypeCatalog(typeof (CascadeDeleteTrigger))));
            db = store.SystemDatabase;
        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanCascadeDeletes()
        {
            db.Documents.Put("abc", null, RavenJObject.Parse("{name: 'a'}"), RavenJObject.Parse("{'Cascade-Delete': 'def'}"), null);
            db.Documents.Put("def", null, RavenJObject.Parse("{name: 'b'}"), new RavenJObject(), null);

            db.Documents.Delete("abc", null, null);

            Assert.Null(db.Documents.Get("def", null));
        }
        
    }
}
