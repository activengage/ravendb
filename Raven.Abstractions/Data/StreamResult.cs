// -----------------------------------------------------------------------
//  <copyright file="StreamResult.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Json.Linq;

namespace Raven35.Abstractions.Data
{
    public class StreamResult<TType>
    {
        /// <summary>
        /// Document key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Document etag.
        /// </summary>
        public Etag Etag { get; set; }

        /// <summary>
        /// Document metadata.
        /// </summary>
        public RavenJObject Metadata { get; set; }

        /// <summary>
        /// Document deserialized to <c>TType</c>.
        /// </summary>
        public TType Document { get; set; }
    }
}
