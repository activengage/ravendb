//-----------------------------------------------------------------------
// <copyright file="IUuidGenerator.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Raven35.Abstractions.Data;

namespace Raven35.Database.Impl
{
    public interface IUuidGenerator
    {
        long LastDocumentTransactionEtag { get; }
        Etag CreateSequentialUuid(UuidType type);
    }
}
