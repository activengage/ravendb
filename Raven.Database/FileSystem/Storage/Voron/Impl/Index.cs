// -----------------------------------------------------------------------
//  <copyright file="Index.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Util.Streams;

namespace Raven35.Database.FileSystem.Storage.Voron.Impl
{
    internal class Index : TableBase
    {
        public Index(string indexName, IBufferPool bufferPool)
            : base(indexName, bufferPool)
        {
        }
    }
}
