//-----------------------------------------------------------------------
// <copyright file="QueryWithReservedCharacters.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Abstractions.Util;
using Raven35.Client.Connection;
using Raven35.Client.Document;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class QueryWithReservedCharacters : RavenTest
    {
        [Fact]
        public void WhenQueryingByGenericClrTypes_ThenAutoQuotedLuceneQueryFails()
        {
            using (var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex(
                    "ByClr",
                    new IndexDefinition
                    {
                        Map = @"from doc in docs select new { ClrType = doc[""@metadata""][""Raven-Clr-Type""] }",
                        Indexes = {{"ClrType", FieldIndexing.NotAnalyzed}},
                    },
                    true);

                using (var session = store.OpenSession())
                {
                    session.Store(new Bar<Foo> {Value = "foo"});

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var typeName = ReflectionUtil.GetFullNameWithoutVersionInformation(typeof (Bar<Foo>));
                    var allSync = session
                        .Advanced
                        .DocumentQuery<Bar<Foo>>("ByClr")
                        .Where("ClrType:[[" + RavenQuery.Escape(typeName,false,false) + "]]")
                        .WaitForNonStaleResultsAsOfNow(TimeSpan.MaxValue)
                        .ToList();

                    Assert.Equal(1, allSync.Count);
                }
            }
        }

        [Fact]
        public void CanQueryWithReservedCharactersWithoutException()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Advanced.DocumentQuery<object>("Raven/DocumentsByEntityName")
                        .Where(RavenQuery.Escape("foo]]]]"))
                        .ToList();
                }
            }
        }

        #region Nested type: Bar

        public class Bar<T>
        {
            public string Value { get; set; }
        }

        #endregion

        #region Nested type: Foo

        public class Foo
        {
        }

        #endregion
    }
}
