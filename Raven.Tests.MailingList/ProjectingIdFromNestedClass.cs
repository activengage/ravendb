// -----------------------------------------------------------------------
//  <copyright file="ProjectingIdFromNestedClass.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class ProjectingIdFromNestedClass : RavenTest
    {

        public class Document
        {
            public string Id
            {
                get;
                set;
            }
        }

        public class Documents_TestIndex : AbstractIndexCreationTask<Document>
        {
            public class Result
            {
                public string Id
                {
                    get;
                    set;
                }
            }

            public Documents_TestIndex()
            {
                Map = docs => from d in docs
                              select new
                              {
                                  d.Id
                              };

                StoreAllFields(FieldStorage.Yes);
            }
        }
 
        [Fact]
        public void TestSelectFields()
        {
            using (var store = NewDocumentStore())
            {
                store.ExecuteIndex(new Documents_TestIndex());

                using (var session = store.OpenSession())
                {
                    session.Store(new Document());
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                                       .DocumentQuery<Document, Documents_TestIndex>()
                                       .WaitForNonStaleResults()
                                       .SelectFields<Documents_TestIndex.Result>()
                                       .ToList();

                    Assert.True(query.All(d => !String.IsNullOrEmpty(d.Id)));
                }
            }
        }
    }
}
