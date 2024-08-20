using System.Xml;
using NLog.Config;
using Raven35.Database.Server;
using Raven35.Tests.Common;

namespace Raven35.Tests
{
    public class WithNLog : NoDisposalNeeded
    {
        static WithNLog()
        {
            if (NLog.LogManager.Configuration != null)
                return;

            HttpEndpointRegistration.RegisterHttpEndpointTarget();

            using (var stream = typeof(WithNLog).Assembly.GetManifestResourceStream("Raven35.Tests.DefaultLogging.config"))
            using (var reader = XmlReader.Create(stream))
            {
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(reader, "default-config");
            }
        }
    }
}
