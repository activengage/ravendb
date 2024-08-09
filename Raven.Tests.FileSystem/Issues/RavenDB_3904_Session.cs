using System.Collections.Generic;
using System.Threading.Tasks;
using Raven35.Abstractions.FileSystem;
using Raven35.Json.Linq;
using Xunit;

namespace Raven35.Tests.FileSystem.Issues
{
    public class RavenDB_3904_Session : RavenFilesTestWithLogs
    {
        [Fact]
        public async Task query_streaming_can_return_more_than_1024_results()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 1500; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("file-" + i, CreateRandomFileStream(2));
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query();

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        int count = 0;

                        while (await reader.MoveNextAsync())
                        {
                            count++;
                        }

                        Assert.Equal(1500, count);
                    }
                }
            }
        }

        [Fact]
        public async Task query_streaming_returns_all_files_if_no_criteria_specified()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 1000; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("text/" + i + ".txt", CreateRandomFileStream(3));
                    await store.AsyncFilesCommands.UploadAsync("binary/" + i + ".bin", CreateRandomFileStream(3));
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query();

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        int allFiles = 0;

                        while (await reader.MoveNextAsync())
                        {
                            allFiles++;
                        }

                        Assert.Equal(2000, allFiles);
                    }
                }
            }
        }

        [Fact]
        public async Task query_streaming_returns_only_files_matching_given_criteria()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 1000; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("text/" + i + ".txt", CreateRandomFileStream(3));
                    await store.AsyncFilesCommands.UploadAsync("binary/" + i + ".bin", CreateRandomFileStream(3));
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().OnDirectory("binary");

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        var allBinaries = 0;

                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal($"/binary/{allBinaries}.bin", reader.Current.FullPath);

                            allBinaries++;
                        }

                        Assert.Equal(1000, allBinaries);
                    }
                }
            }
        }

        [Fact]
        public async Task query_streaming_does_not_return_deleting_files()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 20; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync(i + ".file", CreateRandomFileStream(5));
                    await store.AsyncFilesCommands.UploadAsync(i + ".file", CreateRandomFileStream(5)); // override is going to create .deleting file
                }

                for (int i = 20; i < 33; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync(i + ".file", CreateRandomFileStream(5));
                }

                using (var session = store.OpenAsyncSession())
                {
                    int count = 0;

                    using (var reader = await session.Advanced.StreamQueryAsync(session.Query()))
                    {
                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal($"/{count}.file", reader.Current.FullPath);

                            count++;
                        }
                    }

                    Assert.Equal(33, count);
                }
            }
        }

        [Fact]
        public async Task query_streaming_does_not_include_files_scheduled_for_deletion_nor_tombstones()
        {
            using (var store = this.NewStore())
            {
                using (var session = store.OpenAsyncSession())
                {
                    for (int i = 0; i < 10; i++)
                        session.RegisterUpload(i + ".file", CreateUniformFileStream(10));

                    await session.SaveChangesAsync();
                }

                using (var session = store.OpenAsyncSession())
                {
                    session.RegisterFileDeletion("3.file");

                    await session.SaveChangesAsync();
                }

                int count = 0;
                using (var session = store.OpenAsyncSession())
                {
                    using (var reader = await session.Advanced.StreamQueryAsync(session.Query()))
                    {
                        while (await reader.MoveNextAsync())
                        {
                            count++;
                            Assert.IsType<FileHeader>(reader.Current);
                        }
                    }
                }

                Assert.Equal(9, count);
            }
        }

        [Fact]
        public async Task query_streaming_respects_start_and_pageSize_parameters()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 200; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("file-" + i, CreateRandomFileStream(2));
                }

                var allFiles = new List<FileHeader>();

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().Take(200);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        while (await reader.MoveNextAsync())
                        {
                            allFiles.Add(reader.Current);
                        }

                        Assert.Equal(200, allFiles.Count);
                    }
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().Skip(100).Take(50);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        var count = 0;

                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal(allFiles[100 + count].FullPath, reader.Current.FullPath);

                            count++;
                        }

                        Assert.Equal(50, count);
                    }
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().Skip(150).Take(100);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        var count = 0;

                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal(allFiles[150 + count].FullPath, reader.Current.FullPath);

                            count++;
                        }

                        Assert.Equal(50, count);
                    }
                }
            }
        }

        [Fact]
        public async Task query_streaming_with_criteria_respects_start_and_pageSize_parameters()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 200; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync("file-" + i, CreateRandomFileStream(2), new RavenJObject() { { "Number", i % 3 } });
                }

                var allFilesMatchingCriteria = new List<FileHeader>();

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().WhereEquals("Number", 2);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        while (await reader.MoveNextAsync())
                        {
                            allFilesMatchingCriteria.Add(reader.Current);
                        }

                        Assert.Equal(66, allFilesMatchingCriteria.Count);
                    }
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().WhereEquals("Number", 2).Skip(10).Take(20);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        var count = 0;

                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal(allFilesMatchingCriteria[10 + count].FullPath, reader.Current.FullPath);

                            count++;
                        }

                        Assert.Equal(20, count);
                    }
                }

                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().WhereEquals("Number", 2).Skip(50).Take(100);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        var count = 0;

                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal(allFilesMatchingCriteria[50 + count].FullPath, reader.Current.FullPath);

                            count++;
                        }

                        Assert.Equal(16, count);
                    }
                }
            }
        }

        [Fact]
        public async Task query_streaming_with_sortFields()
        {
            using (var store = NewStore())
            {
                for (int i = 0; i < 10; i++)
                {
                    await store.AsyncFilesCommands.UploadAsync($"{i}.bin", CreateRandomFileStream(2));
                }
                using (var session = store.OpenAsyncSession())
                {
                    var query = session.Query().OrderByDescending(x => x.FullPath);

                    using (var reader = await session.Advanced.StreamQueryAsync(query))
                    {
                        var count = 9;

                        while (await reader.MoveNextAsync())
                        {
                            Assert.Equal($"/{count}.bin", reader.Current.FullPath);

                            count--;
                        }
                    }
                }
            }
        }
    }
}