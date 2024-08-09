// -----------------------------------------------------------------------
//  <copyright file="AggressiveCachingEmbedded.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class AggressiveCachingEmbedded : RavenTest
    {
        [Fact]
        public void CanUseIt()
        {
            using (var store = NewDocumentStore())
            {
                using (store.AggressivelyCache())
                {
                    using (var session = store.OpenSession())
                    {
                        
                    }
                }
            }
        }
    }
}
