//-----------------------------------------------------------------------
// <copyright file="DocumentInTransactionData.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Database.Storage
{
    public class DocumentInTransactionData
    {
        public Etag Etag { get; set; }
        public bool Delete { get; set; }
        public RavenJObject Metadata { get; set; }
        public RavenJObject Data { get; set; }
        public string Key { get; set; }
        public DateTime LastModified { get; set; }

    }
}
