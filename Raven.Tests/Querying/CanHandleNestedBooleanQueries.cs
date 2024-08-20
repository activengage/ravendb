﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Raven35.Abstractions.Data;
using Raven35.Client.Document;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.Querying
{
    public class CanParseNestedBooleanQueries : RavenTest
    {
        private static LuceneASTQueryConfiguration config = new LuceneASTQueryConfiguration
        {
            Analayzer = new RavenPerFieldAnalyzerWrapper(new KeywordAnalyzer()),
            DefaultOperator = QueryOperator.Or,
            FieldName = "foo"
        };

        [Fact]
        void CanParseThreeTermsWithDiffrentOperators()
        {
            var parser = new LuceneQueryParser();
            parser.Parse("foo:a AND foo:b foo:c");
            var query = parser.LuceneAST.ToQuery(config);
            Assert.Equal("+foo:a +foo:b foo:c", query.ToString());
        }

        [Fact]
        void CanParseThreeTermsWithDiffrentOperators2()
        {
            var parser = new LuceneQueryParser();
            parser.Parse("foo:a AND foo:b foo:-c");
            var query = parser.LuceneAST.ToQuery(config);
            Assert.Equal("+foo:a +foo:b foo:c", query.ToString());
        }

        [Fact]
        void CanParseParenthesisInsideNextedBooleanQuery()
        {
            var parser = new LuceneQueryParser();
            parser.Parse("foo:a AND foo:(b -d) foo:-c");
            var query = parser.LuceneAST.ToQuery(config);
            Assert.Equal("+foo:a +(foo:b -foo:d) foo:c", query.ToString());
        }

        [Fact]
        void CanParseParenthesisInsideNextedBooleanQuery2()
        {
            var parser = new LuceneQueryParser();
            parser.Parse("foo:a AND (foo:b -d) foo:-c");
            var query = parser.LuceneAST.ToQuery(config);
            Assert.Equal("+foo:a +(foo:b -foo:d) foo:c", query.ToString());
        }

        [Fact]
        void CanParseComplexedBooleanQuery()
        {
            var parser = new LuceneQueryParser();
            parser.Parse("(foo:a foo:b) (foo:b +d) AND (foo:(e -c) OR g)");
            var query = parser.LuceneAST.ToQuery(config);
            Assert.Equal("(foo:a foo:b) +(foo:b +foo:d) +((foo:e -foo:c) foo:g)", query.ToString());
        }
    }
}
