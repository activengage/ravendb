//-----------------------------------------------------------------------
// <copyright file="SuggestionsLazy.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Linq;
using Raven35.Tests.Bugs;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Suggestions
{
    using System.Collections.Generic;

    public class SuggestionsLazy : RavenTest
    {
        [Fact]
        public void UsingLinq()
        {
            using (GetNewServer())
            using (var store = new DocumentStore { Url = "http://localhost:8079" }.Initialize())
            {
                store.DatabaseCommands.PutIndex("Test", new IndexDefinition
                {
                    Map = "from doc in docs select new { doc.Name }",
                    SuggestionsOptions = new HashSet<string> { "Name" }
                });
                using (var s = store.OpenSession())
                {
                    s.Store(new User { Name = "Ayende" });
                    s.Store(new User { Name = "Oren" });
                    s.SaveChanges();

                    s.Query<User>("Test").Customize(x => x.WaitForNonStaleResults()).ToList();
                }

                using (var s = store.OpenSession())
                {
                    var oldRequests = s.Advanced.NumberOfRequests;

                    var suggestionQueryResult = s.Query<User>("test")
                        .Where(x => x.Name == "Owen")
                        .SuggestLazy();

                    Assert.Equal(oldRequests, s.Advanced.NumberOfRequests);
                    Assert.Equal(1, suggestionQueryResult.Value.Suggestions.Length);
                    Assert.Equal("oren", suggestionQueryResult.Value.Suggestions[0]);

                    Assert.Equal(oldRequests + 1, s.Advanced.NumberOfRequests);
                }
            }
        }
    }
}
