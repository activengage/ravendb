﻿// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3979.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Connection;
using Raven35.Json.Linq;
using Raven35.Bundles.Versioning;
using Raven35.Bundles.Versioning.Triggers;
using Raven35.Database.Config;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bundles.Versioning
{
    public class RavenDB_3979_documents : RavenTest
    {
        private bool changesToRevisionsAllowed = false;

        [Fact]
        public void must_not_allow_to_create_historical_documents_if_changes_to_revisions_are_not_allowed()
        {
            using (var documentStore = NewDocumentStore(activeBundles: "Versioning"))
            {
                using (var session = documentStore.OpenSession())
                {
                    session.Store(new Raven35.Bundles.Versioning.Data.VersioningConfiguration
                    {
                        Exclude = false,
                        Id = Constants.Versioning.RavenVersioningDefaultConfiguration,
                        MaxRevisions = 5
                    });

                    session.SaveChanges();
                }

                var exception = Assert.Throws<ErrorResponseException>(() => documentStore.DatabaseCommands.Put("items/1/revision", null, new RavenJObject(), new RavenJObject() { { VersioningUtil.RavenDocumentRevisionStatus, "Historical" } }));

                Assert.Contains(VersioningPutTrigger.CreationOfHistoricalRevisionIsNotAllowed, exception.Message);
            }
        }

        [Fact]
        public void allows_to_create_historical_documents_if_changes_to_revisions_are_allowed()
        {
            changesToRevisionsAllowed = true;

            using (var documentStore = NewDocumentStore(activeBundles: "Versioning"))
            {
                using (var session = documentStore.OpenSession())
                {
                    session.Store(new Raven35.Bundles.Versioning.Data.VersioningConfiguration
                    {
                        Exclude = false,
                        Id = Constants.Versioning.RavenVersioningDefaultConfiguration,
                        MaxRevisions = 5
                    });

                    session.SaveChanges();
                }

                Assert.DoesNotThrow(() => documentStore.DatabaseCommands.Put("items/1/revision", null, new RavenJObject(), new RavenJObject()
                                                                                             {
                                                                                                { VersioningUtil.RavenDocumentRevisionStatus, "Historical" }
                                                                                             }));
            }
        }

        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            if (changesToRevisionsAllowed)
                configuration.Settings[Constants.Versioning.ChangesToRevisionsAllowed] = "true";
        }
    }
}