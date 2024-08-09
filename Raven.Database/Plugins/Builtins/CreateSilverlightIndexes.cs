using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;

namespace Raven35.Database.Plugins.Builtins
{
    public class CreateSilverlightIndexes : ISilverlightRequestedAware
    {
        public void SilverlightWasRequested(DocumentDatabase database)
        {
            var ravenDocumentsByEntityName = new RavenDocumentsByEntityName {};
            database.Indexes.PutIndex(Constants.DocumentsByEntityNameIndex,
                ravenDocumentsByEntityName.CreateIndexDefinition());
        }
    }
}
