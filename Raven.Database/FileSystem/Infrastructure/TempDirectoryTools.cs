using System.IO;

using Raven35.Database.Config;

namespace Raven35.Database.FileSystem.Infrastructure
{
    public class TempDirectoryTools
    {
        public static string Create(InMemoryRavenConfiguration configuration)
        {
            string tempDirectory;
            do
            {
                tempDirectory = Path.Combine(configuration.TempPath, Path.GetRandomFileName());
            } while (Directory.Exists(tempDirectory));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
