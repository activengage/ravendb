//-----------------------------------------------------------------------
// <copyright file="MissingAnalyzer.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class MissingAnalyzer : RavenTest
    {
        [Fact]
        public void Should_give_clear_error_when_starting()
        {
            using (var store = NewDocumentStore())
            {
                var e = Assert.Throws<IndexCompilationException>(() => store.DatabaseCommands.PutIndex("foo",
                                                                                               new IndexDefinition
                                                                                               {
                                                                                                   Map =
                                                                                                       "from doc in docs select new { doc.Name}",
                                                                                                   Analyzers =
                                                                                                       {
                                                                                                           {
                                                                                                               "Name",
                                                                                                               "foo bar"
                                                                                                               }
                                                                                                       }
                                                                                               }));

                Assert.Equal("Cannot find analyzer type 'foo bar' for field: Name", e.Message);
            }
        }
    }
}
