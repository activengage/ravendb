// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1187.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Connection;
using Raven35.Tests.Common;

namespace Raven35.Tests.Issues
{
    using System;
    using System.Collections.Generic;

    using Raven35.Abstractions.Data;
    using Raven35.Abstractions.Indexing;

    using Xunit;

    public class RavenDB_1187 : RavenTest
    {
        [Fact]
        public void QueryingForSuggestionsAgainstFieldWithSuggestionsTurnedOnShouldNotThrow()
        {
            Assert.DoesNotThrow(() =>
                   {
                       using (var store = this.NewDocumentStore())
                       {
                           store.DatabaseCommands.PutIndex("Test", new IndexDefinition
                           {
                               Map = "from doc in docs select new { doc.Name, doc.Other }",
                               SuggestionsOptions = new HashSet<string> { "Name" }
                           });

                           store.DatabaseCommands.Suggest("Test", new SuggestionQuery
                           {
                               Field = "Name",
                               Term = "Oren",
                               MaxSuggestions = 10,
                           });
                       }
                   });
        }

        [Fact]
        public void QueryingForSuggestionsAgainstFieldWithSuggestionsTurnedOffShouldThrow()
        {
            var e = Assert.Throws<ErrorResponseException>(() =>
                    {
                        using (var store = this.NewDocumentStore())
                        {
                            store.DatabaseCommands.PutIndex("Test", new IndexDefinition
                            {
                                Map = "from doc in docs select new { doc.Name, doc.Other }",
                                SuggestionsOptions = new HashSet<string> { "Name"}
                            });

                            store.DatabaseCommands.Suggest("Test", new SuggestionQuery
                            {
                                Field = "Other",
                                Term = "Oren",
                                MaxSuggestions = 10,
                            });
                        }
                    });

            Assert.Contains("Index 'Test' does not have suggestions configured for field 'Other'.", e.Message);
        }
    }
}
