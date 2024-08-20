// -----------------------------------------------------------------------
//  <copyright file="DamianPutSnapshot.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Embedded;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class DamianPutSnapshot : RavenTest
    {
        [Fact]
        public void Cannot_modify_snapshot()
        {
            using (var documentStore = NewDocumentStore())
            {
                documentStore.Initialize();
                documentStore.SystemDatabase.PutTriggers.Add(new PutTrigger {Database = documentStore.SystemDatabase});
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    session.Store(new Doc {Id = "DocId1", Name = "Name1"});
                    session.SaveChanges();
                }
            }
        }

        public class Doc
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class PutTrigger : AbstractPutTrigger
        {
            public override void OnPut(string key,
                                       RavenJObject jsonReplicationDocument,
                                       RavenJObject metadata,
                                       TransactionInformation transactionInformation)
            {
                using (Database.DisableAllTriggersForCurrentThread())
                {
                    var revisionCopy = new RavenJObject(jsonReplicationDocument);
                    Database.Documents.Put("CopyOfDoc", null, revisionCopy, new RavenJObject(metadata), transactionInformation);
                }
            }
        }
    }
}
