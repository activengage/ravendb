using System;
using System.Collections.Generic;

namespace Raven35.Database.Server.Security
{
    public class IgnoreDb
    {
        public static readonly HashSet<string> Urls = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            // allow to get files not secret if you have access to the server (not for the specific DB)
            "/databases",
            "/fs",
            "/license/status",
            "/license/support",
            "/studio-tasks/server-configs"
        };
    }
}
