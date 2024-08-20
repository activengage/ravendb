// -----------------------------------------------------------------------
//  <copyright file="WithIIS.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using Raven35.Abstractions.Data;
using Raven35.Tests.Bugs.Identifiers;
using Raven35.Tests.Common.Attributes;
using Raven35.Tests.Common.Util;

using Xunit;

namespace Raven35.Tests.Notifications
{
    public class WithIIS : IisExpressTestClient
    {
        public class Item
        {
        }

        [IISExpressInstalledFact]
        public void CheckNotificationInIIS()
        {
            using (var store = NewDocumentStore())
            {
                var list = new BlockingCollection<DocumentChangeNotification>();

                store.Changes().Task.Result
                    .ForDocument("items/1").Task.Result
                    .Subscribe(list.Add);

                using (var session = store.OpenSession())
                {
                    session.Store(new Item(), "items/1");
                    session.SaveChanges();
                }

                DocumentChangeNotification documentChangeNotification;
                Assert.True(list.TryTake(out documentChangeNotification, TimeSpan.FromSeconds(10)));

                Assert.Equal("items/1", documentChangeNotification.Id);
                Assert.Equal(documentChangeNotification.Type, DocumentChangeTypes.Put);
            }
        }
    }
}
