// -----------------------------------------------------------------------
//  <copyright file="TouchFilesTests.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Util;
using Raven35.Json.Linq;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.FileSystem
{
    public class TouchFilesTests : RavenFilesTestBase
    {
        [Fact]
        public async Task CanTouchBatchOfFiles()
        {
            var client = NewAsyncClient();

            try
            {
                await client.DownloadAsync("doesnt-exist");
            }
            catch (Exception)
            {
                // ignore
            }

            var numberOfFiles = 53;

            for (int i = 0; i < numberOfFiles; i++)
            {
                await client.UploadAsync($"/my/files/{i}", new MemoryStream(), new RavenJObject
                {
                    {"Number", i}
                });
            }

            var stats = await client.GetStatisticsAsync();

            var start = Etag.Empty;
            long skipped = 0;
            long modifed = 0;

            while (EtagUtil.IsGreaterThan(stats.LastFileEtag, start))
            {
                var result = await client.TouchFilesAsync(start, 10);

                start = result.LastProcessedFileEtag;

                modifed += result.NumberOfProcessedFiles;
                skipped += result.NumberOfFilteredFiles;
            }

            Assert.True(modifed >= numberOfFiles);
            Assert.Equal(0, skipped);
        }
    }
}