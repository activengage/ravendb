using System;
using System.Linq;
using System.Threading.Tasks;
using Raven35.Abstractions.Replication;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class AggressiveCaching : RavenTest
    {
        public AggressiveCaching()
        {
            using (var server = GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079",
                Conventions =
                {
                    FailoverBehavior = FailoverBehavior.FailImmediately,
                    ShouldAggressiveCacheTrackChanges = false
                }
            }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User());
                    session.SaveChanges();
                }

                WaitForAllRequestsToComplete(server);
                server.Server.ResetNumberOfRequests();
            }
        }

        [Fact]
        public void CanAggressivelyCacheLoads()
        {
            using (var server = GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079",
                Conventions =
                {
                    FailoverBehavior = FailoverBehavior.FailImmediately,
                    ShouldAggressiveCacheTrackChanges = false
                }
            }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User());
                    session.SaveChanges();
                }

                WaitForAllRequestsToComplete(server);
                server.Server.ResetNumberOfRequests();

                for (var i = 0; i < 5; i++)
                {
                    using (var session = store.OpenSession())
                    {
                        using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
                        {
                            session.Load<User>("users/1");
                        }
                    }
                }

                WaitForAllRequestsToComplete(server);
                Assert.Equal(1, server.Server.NumberOfRequests);
            }
        }


        [Fact]
        public async Task CanAggressivelyCacheLoads_Async()
        {
            using (var server = GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079",
                Conventions =
                {
                    FailoverBehavior = FailoverBehavior.FailImmediately,
                    ShouldAggressiveCacheTrackChanges = false
                }
            }.Initialize())
            {
                using (var session = store.OpenAsyncSession())
                {
                    await session.StoreAsync(new User());
                    await session.SaveChangesAsync();
                }

                WaitForAllRequestsToComplete(server);
                server.Server.ResetNumberOfRequests();

                for (var i = 0; i < 5; i++)
                {
                    using (var session = store.OpenAsyncSession())
                    {
                        using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
                        {
                           await  session.LoadAsync<User>("users/1");
                        }
                    }
                }

                WaitForAllRequestsToComplete(server);
                Assert.Equal(1, server.Server.NumberOfRequests);
            }
        }


        [Fact]
        public void CanAggressivelyCacheQueries()
        {
            using (var server = GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079",
                Conventions =
                {
                    FailoverBehavior = FailoverBehavior.FailImmediately,
                    ShouldAggressiveCacheTrackChanges = false
                }
            }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User());
                    session.SaveChanges();
                }

                WaitForAllRequestsToComplete(server);
                server.Server.ResetNumberOfRequests();

                for (int i = 0; i < 5; i++)
                {
                    using (var session = store.OpenSession())
                    {
                        using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
                        {
                            session.Query<User>().ToList();
                        }
                    }

                }

                WaitForAllRequestsToComplete(server);
                Assert.Equal(1, server.Server.NumberOfRequests);
            }
        }

        // TODO: NOTE: I think this test is not complete, since the assertion here is exactly the same as in CanAggressivelyCacheQueries.
        [Fact]
        public void WaitForNonStaleResultsIgnoresAggressiveCaching()
        {
            using (var server = GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079",
                Conventions =
                    {
                        FailoverBehavior = FailoverBehavior.FailImmediately,
                        ShouldAggressiveCacheTrackChanges = false
                    }
            }.Initialize())
            {
                using (var session = store.OpenSession())
                {
                    using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
                    {
                        session.Query<User>()
                        .Customize(x => x.WaitForNonStaleResults())
                            .ToList();
                    }
                }

                WaitForAllRequestsToComplete(server);
                Assert.NotEqual(1, server.Server.NumberOfRequests);
            }
        }
    }
}
