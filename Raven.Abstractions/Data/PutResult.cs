//-----------------------------------------------------------------------
// <copyright file="PutResult.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Raven35.Abstractions.Data
{
    /// <summary>
    /// The result of a PUT operation
    /// </summary>
    public class PutResult
    {
        /// <summary>
        /// Key of the document that was PUT.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Etag of the document after PUT operation.
        /// </summary>
        public Etag ETag { get; set; }
    }
}
