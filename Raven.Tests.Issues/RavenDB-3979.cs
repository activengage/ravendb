using System;
using System.IO;
using System.Threading.Tasks;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.Smuggler;
using Raven35.Bundles.Versioning.Data;
using Raven35.Database.FileSystem.Bundles.Versioning;
using Raven35.Smuggler;
using Raven35.Tests.Common;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3979 : RavenFilesTestBase
    {
        [Fact]
        public async Task enableVersioningDuringImportFs()
        {
            var export = Path.Combine(NewDataPath("src"), "Export");

            using (var store = NewStore(activeBundles: "Versioning"))
            {
                await store.AsyncFilesCommands.Configuration.SetKeyAsync(VersioningUtil.DefaultConfigurationName, new VersioningConfiguration
                {
                    Id = VersioningUtil.DefaultConfigurationName
                });

                for (int i = 0; i < 10; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("test-" + i, StringToStream("hello"));
                }

                var options = new FilesConnectionStringOptions
                {
                    Url = store.Url,
                    DefaultFileSystem = store.DefaultFileSystem

                };

                var smuggler = new SmugglerFilesApi();
                await smuggler.ExportData(new SmugglerExportOptions<FilesConnectionStringOptions>
                {
                    From = options,
                    ToFile = export
                });
            }
            using (var store = NewStore(activeBundles: "Versioning", fileSystemName: "Import", index: 1))
            {
                await store.AsyncFilesCommands.Configuration.SetKeyAsync(VersioningUtil.DefaultConfigurationName, new VersioningConfiguration
                {
                    Id = VersioningUtil.DefaultConfigurationName
                });

                var smuggler = new SmugglerFilesApi(new SmugglerFilesOptions()
                {
                    ShouldDisableVersioningBundle = false
                });

                var options = new FilesConnectionStringOptions { Url = store.Url, DefaultFileSystem = store.DefaultFileSystem };

                var e = await AssertAsync.Throws<ErrorResponseException>(async () => await smuggler.ImportData(new SmugglerImportOptions<FilesConnectionStringOptions> { FromFile = export, To = options }));

                Assert.Contains("PUT vetoed on file /test-0/revisions/1 by Raven35.Database.FileSystem.Bundles.Versioning.Plugins.VersioningPutTrigger because: Creating a historical revision is not allowed", e.Message);

            }
        }

        [Fact]
        public async Task disableVersioningDuringImportFs()
        {
            var export = Path.Combine(NewDataPath("src"), "Export");

            using (var store = NewStore(activeBundles: "Versioning"))
            {
                await store.AsyncFilesCommands.Configuration.SetKeyAsync(VersioningUtil.DefaultConfigurationName, new VersioningConfiguration
                {
                    Id = VersioningUtil.DefaultConfigurationName
                });

                for (int i = 0; i < 10; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("test-" + i, StringToStream("hello"));
                }

                var options = new FilesConnectionStringOptions
                {
                    Url = store.Url,
                    DefaultFileSystem = store.DefaultFileSystem

                };

                var smuggler = new SmugglerFilesApi();
                await smuggler.ExportData(new SmugglerExportOptions<FilesConnectionStringOptions>
                {
                    From = options,
                    ToFile = export
                });
            }
            using (var store = NewStore(activeBundles: "Versioning", fileSystemName: "Import", index: 1))
            {
                await store.AsyncFilesCommands.Configuration.SetKeyAsync(VersioningUtil.DefaultConfigurationName, new VersioningConfiguration
                {
                    Id = VersioningUtil.DefaultConfigurationName
                });

                var smuggler = new SmugglerFilesApi(new SmugglerFilesOptions()
                {
                    ShouldDisableVersioningBundle = true
                });

                var options = new FilesConnectionStringOptions { Url = store.Url, DefaultFileSystem = store.DefaultFileSystem };

                var e = await AssertAsync.DoesNotThrow(async () => await smuggler.ImportData(new SmugglerImportOptions<FilesConnectionStringOptions> { FromFile = export, To = options }));

                Assert.True(e);

            }
        }
    }
}
