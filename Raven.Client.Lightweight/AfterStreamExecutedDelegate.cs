using Raven35.Json.Linq;

namespace Raven35.Client
{
    public delegate bool AfterStreamExecutedDelegate(ref RavenJObject document);
}
