using System;
using System.Diagnostics;
using System.IO;

namespace Raven35.Tests.Common.Util
{
    public class IISExpressDriver : ProcessDriver
    {
        public string Url { get; private set;  }

        public void Start(string physicalPath, int port)
        {
            var sitePhysicalDirectory = GetPathParameter(physicalPath);

            foreach (var process in Process.GetProcessesByName("iisexpress"))
            {
                process.Kill();
            }
            
            var processFileName =
                 @"c:\program files\IIS Express\IISExpress.exe";
            StartProcess(processFileName, @"/systray:false /trace:error /port:" + port + @" /path:" + sitePhysicalDirectory);

            var match = WaitForConsoleOutputMatching(@"Successfully registered URL ""([^""]*)""");

            Url = match.Groups[1].Value;
        }

        private static string GetPathParameter(string path)
        {
            if (path == null)
                return null;

            var trimmedPath = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return $"\"{trimmedPath}\"";
        }

        protected override void Shutdown()
        {
            try
            {
                _process.Kill();
            }
            catch (Exception)
            {
            }

            if (!_process.WaitForExit(10000))
                throw new Exception("IISExpress did not halt within 10 seconds.");
        }
    }
}
