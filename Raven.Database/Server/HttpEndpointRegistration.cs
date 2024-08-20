using Raven35.Abstractions.Logging;
using Raven35.Database.Server.Connections;
using Raven35.Database.Util;

namespace Raven35.Database.Server
{
    public static class HttpEndpointRegistration
    {
        public static void RegisterHttpEndpointTarget()
        {
            LogManager.RegisterTarget<DatabaseMemoryTarget>();
        }

        public static void RegisterAdminLogsTarget()
        {
            LogManager.RegisterTarget<AdminLogsTarget>();
        }
    }
}
