// -----------------------------------------------------------------------
//  <copyright file="WritingDocumentsInfo.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;

namespace Raven35.Database.Indexing
{
    public class IndexedItemsInfo
    {
        public IndexedItemsInfo(Etag highestEtag)
        {
            HighestETag = highestEtag;
        }

        public int ChangedDocs { get; set; }

        public Etag HighestETag { get; private set; }

        public string[] DeletedKeys { get; set; }

        public bool DisableCommitPoint { get; set; }
    }
}
