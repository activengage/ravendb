// -----------------------------------------------------------------------
//  <copyright file="QueryResultsStreaming.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class Posts_Recurse : AbstractIndexCreationTask<Post>
    {
        public Posts_Recurse()
        {
            Map = posts => from post in posts
                           from comment in Recurse(post, x => x.Comments)
                           select new 
                           { 
                               Title = post.Title,
                               Desc = post.Desc
                           };
        }
    }
}
