// -----------------------------------------------------------------------
//  <copyright file="ChangedDoc.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;

namespace Raven35.Database.Impl.DTC
{
    public class ChangedDoc
    {
        public string transactionId;
        public Etag currentEtag;
        public Etag committedEtag;
    }
}