// -----------------------------------------------------------------------
//  <copyright file="Index.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Util.Streams;
using Raven35.Database.Util.Streams;

namespace Raven35.Database.Storage.Voron.Impl
{
    internal class Index : TableBase
    {
        public Index(string indexName, IBufferPool bufferPool)
            : base(indexName, bufferPool)
        {
        }
    }
}
