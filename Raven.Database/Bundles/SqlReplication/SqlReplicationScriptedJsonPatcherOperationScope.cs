using Jint.Native;

using Raven35.Database.Json;
using Raven35.Json.Linq;

namespace Raven35.Database.Bundles.SqlReplication
{
    internal class SqlReplicationScriptedJsonPatcherOperationScope : DefaultScriptedJsonPatcherOperationScope
    {
        public SqlReplicationScriptedJsonPatcherOperationScope(DocumentDatabase database)
            : base(database, false)
        {
        }

        public override RavenJObject ConvertReturnValue(JsValue jsObject)
        {
            return null;// we don't use / need the return value
        }
    }
}
