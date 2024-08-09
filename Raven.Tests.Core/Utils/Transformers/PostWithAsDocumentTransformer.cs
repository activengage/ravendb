// -----------------------------------------------------------------------
//  <copyright file="PostWithAsDocumentTransformer.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;
using Raven35.Client.Indexes;
using Raven35.Json.Linq;
using Raven35.Tests.Core.Utils.Entities;

namespace Raven35.Tests.Core.Utils.Transformers
{
    public class PostWithAsDocumentTransformer : AbstractTransformerCreationTask<Post>
    {
        public class Result
        {
            public RavenJObject RawDocument { get; set; }
        }

        public PostWithAsDocumentTransformer()
        {
            TransformResults = posts => from post in posts
                                        select new Result
                                        {
                                            RawDocument = AsDocument(post)
                                        };
        }
    }
}
