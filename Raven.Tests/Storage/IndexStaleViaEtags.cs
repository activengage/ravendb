//-----------------------------------------------------------------------
// <copyright file="IndexStaleViaEtags.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Threading;
using Raven35.Client.Embedded;
using Raven35.Json.Linq;
using Raven35.Client.Indexes;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Storage
{
    public class IndexStaleViaEtags : RavenTest
    {
        private readonly EmbeddableDocumentStore store;
        private readonly DocumentDatabase db;
        private int entityNameId = 0;


        public IndexStaleViaEtags()
        {
            store = NewDocumentStore();
            db = store.SystemDatabase;
            db.Indexes.PutIndex(new RavenDocumentsByEntityName().IndexName, new RavenDocumentsByEntityName().CreateIndexDefinition());
            entityNameId = db.IndexDefinitionStorage.GetIndexDefinition(new RavenDocumentsByEntityName().IndexName).IndexId;

        }

        public override void Dispose()
        {
            store.Dispose();
            base.Dispose();
        }

        [Fact]
        public void CanTellThatIndexIsStale()
        {
            db.TransactionalStorage.Batch(accessor => Assert.False(accessor.Staleness.IsIndexStale(entityNameId, null, null)));

            db.Documents.Put("ayende", null, new RavenJObject(), new RavenJObject(), null);

            db.TransactionalStorage.Batch(accessor => Assert.True(accessor.Staleness.IsIndexStale(entityNameId, null, null)));
        }

        [Fact]
        public void CanIndexDocuments()
        {
            db.TransactionalStorage.Batch(accessor => Assert.False(accessor.Staleness.IsIndexStale(entityNameId, null, null)));

            db.Documents.Put("ayende", null, new RavenJObject(), new RavenJObject(), null);

            bool indexed = false;
            for (int i = 0; i < 500; i++)
            {
                db.TransactionalStorage.Batch(accessor => indexed = (accessor.Staleness.IsIndexStale(entityNameId, null, null)));
                if (indexed == false)
                    break;
                Thread.Sleep(50);
            }

            Assert.False(indexed);
        }
    }
}
