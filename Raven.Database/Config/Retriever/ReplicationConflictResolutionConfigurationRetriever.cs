using System.IO;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;

namespace Raven35.Database.Config.Retriever
{
    internal class ReplicationConflictResolutionConfigurationRetriever : ConfigurationRetrieverBase<ReplicationConfig>
    {
        protected override ReplicationConfig ApplyGlobalDocumentToLocal(ReplicationConfig global, ReplicationConfig local, DocumentDatabase systemDatabase, DocumentDatabase localDatabase)
        {
            return local;
        }

        protected override ReplicationConfig ConvertGlobalDocumentToLocal(ReplicationConfig global, DocumentDatabase systemDatabase, DocumentDatabase localDatabase)
        {
            return global;
        }

        public override string GetGlobalConfigurationDocumentKey(string key, DocumentDatabase systemDatabase, DocumentDatabase localDatabase)
        {
            return Constants.Global.ReplicationConflictResolutionDocumentName;
        }
    }
}
