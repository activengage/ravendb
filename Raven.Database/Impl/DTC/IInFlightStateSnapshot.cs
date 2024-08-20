using System;
using Raven35.Abstractions.Data;

namespace Raven35.Database.Impl.DTC
{
    public interface IInFlightStateSnapshot
    {
        Func<TDocument, TDocument> GetNonAuthoritativeInformationBehavior<TDocument>(TransactionInformation tx, string key) where TDocument : class, IJsonDocumentMetadata, new();
    }
}