using System;
using Raven35.Json.Linq;

namespace Raven35.Abstractions.Streaming
{
    public interface IOutputWriter : IDisposable
    {
        string ContentType { get; }

        void WriteHeader();
        void Write(RavenJObject result);
        void WriteError(Exception exception);
        void Flush();
    }
}
