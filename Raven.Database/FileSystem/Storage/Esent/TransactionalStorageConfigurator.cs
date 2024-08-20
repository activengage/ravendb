using Raven35.Database.Config;
using Raven35.Database.Storage.Esent;

namespace Raven35.Database.FileSystem.Storage.Esent
{
    public class TransactionalStorageConfigurator : StorageConfigurator
    {
        public TransactionalStorageConfigurator(InMemoryRavenConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void ConfigureInstanceInternal(int maxVerPages)
        {
            // nothing to do here
        }

        protected override string BaseName
        {
            get
            {
                return "RFS";
            }
        }

        protected override string EventSource
        {
            get
            {
                return "RavenFS";
            }
        }
    }
}
