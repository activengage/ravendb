// -----------------------------------------------------------------------
//  <copyright file="BulkInsertWithChanges.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Client.Document;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;
using Xunit;
using System;

namespace Raven35.Tests.MailingList
{
    public class BulkInsertWithChanges : RavenTest
    {
        [Fact]
        public void StartsWithChangesThrowsWithBulkInsert()
        {
            using (GetNewServer())
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                Exception e = null;
                store.Changes().ForDocumentsStartingWith("something").Subscribe(notification => { }, exception => e = exception);

                using (var session = store.BulkInsert())
                {
                    session.Store(new Company(), "else/1");
                }

                Assert.Null(e);
            }
        } 
    }
}
