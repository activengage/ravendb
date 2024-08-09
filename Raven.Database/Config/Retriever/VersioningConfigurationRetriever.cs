using System;

using Raven35.Abstractions.Data;
using Raven35.Bundles.Versioning.Data;

namespace Raven35.Database.Config.Retriever
{
    public class VersioningConfigurationRetriever : ConfigurationRetrieverBase<VersioningConfiguration>
    {
        protected override VersioningConfiguration ApplyGlobalDocumentToLocal(VersioningConfiguration global, VersioningConfiguration local, DocumentDatabase systemDatabase, DocumentDatabase localDatabase)
        {
            return local;
        }

        protected override VersioningConfiguration ConvertGlobalDocumentToLocal(VersioningConfiguration global, DocumentDatabase systemDatabase, DocumentDatabase localDatabase)
        {
            if (string.IsNullOrEmpty(global.Id) == false) 
                global.Id = global.Id.Replace(Constants.Global.VersioningDocumentPrefix, Constants.Versioning.RavenVersioningPrefix);

            return global;
        }

        public override string GetGlobalConfigurationDocumentKey(string key, DocumentDatabase systemDatabase, DocumentDatabase localDatabase)
        {
            if (string.Equals(key, Constants.Versioning.RavenVersioningDefaultConfiguration, StringComparison.OrdinalIgnoreCase))
                return Constants.Global.VersioningDefaultConfigurationDocumentName;

            if (key.StartsWith(Constants.Versioning.RavenVersioningPrefix, StringComparison.OrdinalIgnoreCase))
                return key.Replace(Constants.Versioning.RavenVersioningPrefix, Constants.Global.VersioningDocumentPrefix);

            throw new NotSupportedException("Not supported key: " + key);
        }
    }
}
