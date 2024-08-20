using System;

namespace Raven35.Database.Plugins.Builtins
{
    public class InvalidDocumentNames : AbstractPutTrigger
    {
        public override VetoResult AllowPut(string key, Raven35.Json.Linq.RavenJObject document, Raven35.Json.Linq.RavenJObject metadata, Abstractions.Data.TransactionInformation transactionInformation)
        {
            if(key.Contains(@"\"))
                return VetoResult.Deny(@"Document name cannot contain '\' but attempted to save with: " + key);
            if(string.Equals(key, "Raven35.Databases/System", StringComparison.OrdinalIgnoreCase))
                return
                    VetoResult.Deny(
                        @"Cannot create a tenant database with the name 'System', that name is reserved for the actual system database");

            return VetoResult.Allowed;
        }
    }
}
