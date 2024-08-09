// -----------------------------------------------------------------------
//  <copyright file="CounterChanges.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Raven35.Imports.Newtonsoft.Json;

namespace Raven35.Abstractions.Counters
{
    public class CounterChange
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public long Delta { get; set; }

        public bool IsReset { get; set; }

        [JsonIgnore]
        public TaskCompletionSource<object> Done { get; set; }
    }
}
