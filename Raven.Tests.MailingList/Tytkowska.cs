﻿// -----------------------------------------------------------------------
//  <copyright file="Tytkowska.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Tytkowska : RavenTestBase
    {
        public class ExampleModel
        {
            public string Id;
            public string SearchString;
        }

        public class ExampleIndex : AbstractIndexCreationTask<ExampleModel>
        {
            public ExampleIndex()
            {
                Map = examples => from example in examples
                    select new
                    {
                        Id = example.Id,
                        SearchString = example.SearchString
                    };
                Index(x => x.SearchString, FieldIndexing.Analyzed);
            }
        }


        [Fact]
        public void NoResultTestMethod()
        {
            using (DocumentStore store = NewRemoteDocumentStore())
            {
                new ExampleIndex().Execute(store);

                using (var session = store.OpenSession())
                {
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/90ce55c0-d3a8-4982-af33-85a7d525ae01",
                        Id = "a"
                    });
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/3be3bdbc-34a1-48d4-aa28-de5bc873d510",
                        Id = "b"
                    });
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/2f7392c1-dc2b-4199-9e40-4d4fc6758c92",
                        Id = "c"
                    });
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/a04abe17-301a-4a90-b1ce-a6bf7590dd5f",
                        Id = "d"
                    });
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/8f6ff28c-6923-4737-b0c1-6e2b7134036",
                        Id = "e"
                    });
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/6",
                        Id = "f"
                    });
                    session.Store(new ExampleModel
                    {
                        SearchString = "ExampleModels/7",
                        Id = "g"
                    });
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                using (var session = store.OpenSession())
                {
                    var ids = new List<string> {"ExampleModels/90ce55c0-d3a8-4982-af33-85a7d525ae01", "ExampleModels/3be3bdbc-34a1-48d4-aa28-de5bc873d510", "ExampleModels/2f7392c1-dc2b-4199-9e40-4d4fc6758c92", "ExampleModels/a04abe17-301a-4a90-b1ce-a6bf7590dd5f", "ExampleModels/8f6ff28c-6923-4737-b0c1-6e2b7134036"};
                    var examples1Query = session.Advanced.DocumentQuery<ExampleModel, ExampleIndex>().WhereIn(x => x.SearchString, ids);
                    var examples1 = examples1Query.ToList();

                    ids.Add("ExampleModels/6");
                    var examples2Query = session.Advanced.DocumentQuery<ExampleModel, ExampleIndex>().WhereIn(x => x.SearchString, ids);
                    var examples2 = examples2Query.ToList();

                    Assert.Equal(5, examples1.Count);
                    Assert.Equal(6, examples2.Count);
                }
            }
        }
    }
}