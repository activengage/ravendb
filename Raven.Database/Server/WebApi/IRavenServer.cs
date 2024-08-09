using Raven35.Database.Config;

namespace Raven35.Database.Server.WebApi
{
    public interface IRavenServer
    {
        DocumentDatabase SystemDatabase { get; }

        InMemoryRavenConfiguration SystemConfiguration { get; }
    }
}
