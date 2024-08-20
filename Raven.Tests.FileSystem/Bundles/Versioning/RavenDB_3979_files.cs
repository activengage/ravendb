// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3979.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Database.Bundles.Versioning.Data;
using Raven35.Database.Config;
using Raven35.Database.FileSystem.Bundles.Versioning;
using Raven35.Database.FileSystem.Bundles.Versioning.Plugins;
using Raven35.Json.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Helpers;

using Xunit;

namespace Raven35.Tests.FileSystem.Bundles.Versioning
{
    public class RavenDB_3979_files : RavenFilesTestBase
    {
        [Fact]
        public async Task must_not_allow_to_create_historical_file_if_changes_to_revisions_are_not_allowed()
        {
            using (var store = NewStore(activeBundles: "Versioning"))
            {
                await store.AsyncFilesCommands.Configuration.SetKeyAsync(VersioningUtil.DefaultConfigurationName, new FileVersioningConfiguration { Id = VersioningUtil.DefaultConfigurationName, MaxRevisions = 10 });

                var exception = await AssertAsync.Throws<ErrorResponseException>(() => store.AsyncFilesCommands.UploadAsync("files/1/revision", new MemoryStream(), new RavenJObject() { { VersioningUtil.RavenFileRevisionStatus, "Historical" } }));

                Assert.Contains(VersioningTriggerActions.CreationOfHistoricalRevisionIsNotAllowed, exception.Message);
            }
        }

        [Fact]
        public async Task allows_to_create_historical_file_if_changes_to_revisions_are_allowed()
        {
            using (var store = NewStore(activeBundles: "Versioning", customConfig: configuration => configuration.Settings[Constants.FileSystem.Versioning.ChangesToRevisionsAllowed] = "true"))
            {
                await store.AsyncFilesCommands.Configuration.SetKeyAsync(VersioningUtil.DefaultConfigurationName, new FileVersioningConfiguration { Id = VersioningUtil.DefaultConfigurationName, MaxRevisions = 10 });


                Assert.True(await AssertAsync.DoesNotThrow(() => store.AsyncFilesCommands.UploadAsync("files/1/revision", new MemoryStream(), new RavenJObject() { { VersioningUtil.RavenFileRevisionStatus, "Historical" } })));
            }
        }
    }
}