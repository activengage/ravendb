using Raven35.Abstractions.Data;

namespace Raven35.Client.Util
{
    public interface ILastEtagHolder
    {
        void UpdateLastWrittenEtag(Etag etag);
        Etag GetLastWrittenEtag();
    }
}
