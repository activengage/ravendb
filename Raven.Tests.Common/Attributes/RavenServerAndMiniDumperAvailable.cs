using System;
using System.Collections.Generic;
using System.IO;

using Xunit;
using Xunit.Sdk;


namespace Raven35.Tests.Common.Attributes
{

    [CLSCompliant(false)]
    public class RavenServerAndMiniDumperAvailable : FactAttribute
    {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            var displayName = method.TypeName + "." + method.Name;

            if (LookForRavenPaths() == false)
            {
                yield return
                    new SkipCommand(method, displayName,
                        "Could not execute " + displayName + " because it requires Raven35.Server.exe and Raven35.MiniDumper.exe");
                yield break;
            }

            foreach (var command in base.EnumerateTestCommands(method))
            {
                yield return command;
            }
        }

        private bool LookForRavenPaths()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var expectedRavenPath = Path.Combine(baseDir, @"..\..\..\Raven35.Server\bin\Debug\Raven35.Server.exe");
            if (File.Exists(expectedRavenPath))
            {
                RavenServerPath = expectedRavenPath;
            }
            else
            {
                expectedRavenPath = Path.Combine(baseDir, @"..\..\..\Raven35.Server\bin\Release\Raven35.Server.exe");
                if (File.Exists(expectedRavenPath))
                    RavenServerPath = expectedRavenPath;
                else return false;
            }
            var expectedMiniDumperPath = Path.Combine(baseDir, @"..\..\..\Raven35.MiniDumper\bin\Debug\Raven35.MiniDumper.exe");
            if (File.Exists(expectedMiniDumperPath))
                MiniDumperPath = expectedMiniDumperPath;
            else
            {
                expectedMiniDumperPath = Path.Combine(baseDir, @"..\..\..\Raven35.MiniDumper\bin\Release\Raven35.MiniDumper.exe");
                if (File.Exists(expectedMiniDumperPath))
                    MiniDumperPath = expectedMiniDumperPath;
                else return false;
            }
            var ravenBaseDir = Path.GetDirectoryName(expectedRavenPath);
            LocalConfigPath = Path.Combine(ravenBaseDir, "local.config");
            //should not happen
            if (!File.Exists(LocalConfigPath))
                return false;
            return true;
        }

        public static string RavenServerPath { get; set; }
        public static string MiniDumperPath { get; set; }
        public static string LocalConfigPath { get; set; }
    }
    
}
