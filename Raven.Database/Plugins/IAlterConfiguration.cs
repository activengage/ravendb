using System.ComponentModel.Composition;
using Raven35.Database.Config;

namespace Raven35.Database.Plugins
{
    [InheritedExport]
    public interface IAlterConfiguration
    {
        void AlterConfiguration(InMemoryRavenConfiguration configuration);
    }
}
