using System.Xml;
using NLog;
using NLog.Config;
using Raven35.Database.Server;
using Raven35.Tests.Helpers;

namespace Raven35.Tests.FileSystem
{
    public class RavenFilesTestWithLogs : RavenFilesTestBase
    {
        static RavenFilesTestWithLogs()
        {
            if (LogManager.Configuration != null)
                return;

            HttpEndpointRegistration.RegisterHttpEndpointTarget();

            using (var stream = typeof(RavenFilesTestWithLogs).Assembly.GetManifestResourceStream("Raven35.Tests.FileSystem.DefaultLogging.config"))
            using (var reader = XmlReader.Create(stream))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(reader, "default-config");
            }
        }
    }
}
