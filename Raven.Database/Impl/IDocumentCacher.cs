using System;
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Database.Impl
{
    public interface IDocumentCacher : IDisposable
    {
        CachedDocument GetCachedDocument(string key, Etag etag, bool metadataOnly = false);
        void SetCachedDocument(string key, Etag etag, RavenJObject doc, RavenJObject metadata, int size);
        void RemoveCachedDocument(string key, Etag etag);
        object GetStatistics();
    }
}
