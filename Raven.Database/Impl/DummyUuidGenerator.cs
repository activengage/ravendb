//-----------------------------------------------------------------------
// <copyright file="DummyUuidGenerator.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Data;

namespace Raven35.Database.Impl
{
    public class DummyUuidGenerator : IUuidGenerator
    {
        private byte cur;
        public long LastDocumentTransactionEtag
        {
            get { return -1; }
        }

        public Etag CreateSequentialUuid(UuidType type)
        {
            var bytes = new byte[16];
            bytes[15] += ++cur;
            return Etag.Parse(bytes);
        }
    }
}
