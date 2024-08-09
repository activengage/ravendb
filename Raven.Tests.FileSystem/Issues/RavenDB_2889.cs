// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2889.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Threading.Tasks;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.FileSystem;
using Raven35.Abstractions.Smuggler;
using Raven35.Database.Extensions;
using Raven35.Json.Linq;
using Raven35.Smuggler;
using Raven35.Tests.Common;
using Raven35.Tests.FileSystem.Synchronization;

using Xunit;

namespace Raven35.Tests.FileSystem.Issues
{
    public class RavenDB_2889 : RavenFilesTestWithLogs
    {
        [Fact]
        public async Task SmugglerCanStripReplicationInformationDuringImport()
        {
            using (var store = NewStore())
            {
                var server = GetServer();
                var outputDirectory = Path.Combine(server.Configuration.DataDirectory, "Export");

                try
                {
                    await store.AsyncFilesCommands.Admin.EnsureFileSystemExistsAsync("N1");
                    await store.AsyncFilesCommands.Admin.EnsureFileSystemExistsAsync("N2");

                    var content = new MemoryStream(new byte[] { 1, 2, 3 })
                    {
                        Position = 0
                    };
                    var commands = store.AsyncFilesCommands.ForFileSystem("N1");
                    await commands.UploadAsync("test.bin", content, new RavenJObject { { "test", "value" } });
                    var metadata = await commands.GetMetadataForAsync("test.bin");
                    var source1 = metadata[SynchronizationConstants.RavenSynchronizationSource];
                    var version1 = metadata[SynchronizationConstants.RavenSynchronizationVersion];
                    var history1 = metadata[SynchronizationConstants.RavenSynchronizationHistory] as RavenJArray;
                    Assert.NotNull(source1);
                    Assert.NotNull(version1);
                    Assert.NotNull(history1);
                    Assert.Empty(history1);

                    var smugglerApi = new SmugglerFilesApi(new SmugglerFilesOptions { StripReplicationInformation = true });
                    var export = await smugglerApi.ExportData(new SmugglerExportOptions<FilesConnectionStringOptions> { From = new FilesConnectionStringOptions { Url = store.Url, DefaultFileSystem = "N1" }, ToFile = outputDirectory });
                    await smugglerApi.ImportData(new SmugglerImportOptions<FilesConnectionStringOptions> { FromFile = export.FilePath, To = new FilesConnectionStringOptions { Url = store.Url, DefaultFileSystem = "N2" } });

                    commands = store.AsyncFilesCommands.ForFileSystem("N2");
                    metadata = await commands.GetMetadataForAsync("test.bin");
                    var source2 = metadata[SynchronizationConstants.RavenSynchronizationSource];
                    var version2 = metadata[SynchronizationConstants.RavenSynchronizationVersion];
                    var history2 = metadata[SynchronizationConstants.RavenSynchronizationHistory] as RavenJArray;
                    Assert.NotEqual(source1, source2);
                    Assert.Equal(version1, version2);
                    Assert.NotNull(history2);
                    Assert.Empty(history2);
                }
                finally
                {
                    IOExtensions.DeleteDirectory(outputDirectory);
                }
            }
        }
    }
}
