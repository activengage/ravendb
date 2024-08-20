// -----------------------------------------------------------------------
//  <copyright file="CommentsInQueries.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using Lucene.Net.Analysis;
using Raven35.Database.Indexing;
using Xunit;

namespace Raven35.Tests.Queries
{
    public class CommentsInQueries : IDisposable
    {
        [Fact]
        public void ShouldBeSafelyIgnored()
        {
            var query = QueryBuilder.BuildQuery(@"Hi: There mister // comment

Hi: ""where // are "" // comment

Be: http\://localhost\:8080

",new RavenPerFieldAnalyzerWrapper(new KeywordAnalyzer()));

            var s = query.ToString();

            Assert.DoesNotContain("comment", s);
        }

        public void Dispose()
        {
            
        }
    }
}
