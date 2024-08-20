// -----------------------------------------------------------------------
//  <copyright file="QueryResultWithIncludes.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using Raven35.Abstractions.Data;
using Raven35.Imports.Newtonsoft.Json;

namespace Raven35.Database.Data
{
    public class QueryResultWithIncludes : QueryResult
    {
        [JsonIgnore]
        public HashSet<string> IdsToInclude { get; set; }
    }
}
