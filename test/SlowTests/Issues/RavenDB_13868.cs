﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastTests;
using FastTests.Server.Basic.Entities;
using Sparrow.Server;
using Xunit;

namespace SlowTests.Issues
{
    public class RavenDB_13868 : RavenTestBase
    {
        private readonly TimeSpan _reasonableWaitTime = Debugger.IsAttached ? TimeSpan.FromSeconds(60 * 10) : TimeSpan.FromSeconds(30);
        [Fact]
        public async Task CollectionInSubscriptionsShouldbeCaseInsensitive()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new Order()
                    {
                        Employee = "zzz"
                    });
                    session.SaveChanges();
                }

                var subsId = store.Subscriptions.Create(new Raven.Client.Documents.Subscriptions.SubscriptionCreationOptions()
                {
                    Query = @"from orders as o where o.Employee=='zzz'"
                });
                var subsWorker = store.Subscriptions.GetSubscriptionWorker<Order>(new Raven.Client.Documents.Subscriptions.SubscriptionWorkerOptions(subsId)
                {
                    MaxDocsPerBatch = 1
                });

                var amre = new AsyncManualResetEvent();
                subsWorker.AfterAcknowledgment += batch =>
                {
                    amre.Set();
                    return Task.CompletedTask;
                };
                GC.KeepAlive(subsWorker.Run(x => { }));
                Assert.True(await amre.WaitAsync(_reasonableWaitTime));
                amre.Reset();
                for (int i = 0; i < 9; i++)
                {
                    using (var session = store.OpenSession())
                    {
                        session.Store(new Order()
                        {
                            Employee = "zzz"
                        });
                        session.SaveChanges();
                    }
                    Assert.True(await amre.WaitAsync(_reasonableWaitTime));
                    amre.Reset();
                }
            }
        }
    }

}
