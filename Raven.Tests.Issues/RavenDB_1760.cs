// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1760.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Exceptions;
using Raven35.Json.Linq;
using Raven35.Tests.Bundles;
using Raven35.Tests.Bundles.Replication;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_1760 : ReplicationBase
    {
        [Fact]
        public void LoadStartingWithShouldReturnConflictException_Remote()
        {
            using (var store1 = CreateStore())
            using (var store2 = CreateStore())
            {
                DoTest(store1, store2);
            }
        }

        [Fact]
        public void LoadStartingWithShouldReturnConflictException_Embedded()
        {
            using (var store1 = CreateEmbeddableStore())
            using (var store2 = CreateEmbeddableStore())
            {
                DoTest(store1, store2);
            }
        }

        private void DoTest(IDocumentStore store1, IDocumentStore store2)
        {
            using (var session = store1.OpenSession())
            {
                session.Store(new Company());
                session.SaveChanges();
            }

            using (var session = store2.OpenSession())
            {
                session.Store(new Company {Name = "Company2"});
                session.SaveChanges();
            }

            store1.DatabaseCommands.Put("marker", null, new RavenJObject(), new RavenJObject());

            TellFirstInstanceToReplicateToSecondInstance();

            WaitForReplication(store2, "marker");

            var conflictException = Assert.Throws<ConflictException>(() =>
            {
                using (var session = store2.OpenSession())
                {
                    var loadStartingWith = session.Advanced.LoadStartingWith<Company>("companies/");
                }
            });

            Assert.Equal("Conflict detected on companies/1, conflict must be resolved before the document will be accessible",
                         conflictException.Message);
        }
    }
}
