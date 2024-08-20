// -----------------------------------------------------------------------
//  <copyright file="?.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace Raven35.Database.Storage
{
    public class DocCountWithSampleDocIds
    {
        public int Count { get; set; }
        public HashSet<string> SampleDocsIds { get; set; }
    }
}
