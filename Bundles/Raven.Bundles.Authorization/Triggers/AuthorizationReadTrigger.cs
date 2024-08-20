//-----------------------------------------------------------------------
// <copyright file="AuthorizationReadTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition;
using System.IO;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Database.Server;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Authorization.Triggers
{
    [InheritedExport(typeof(AbstractReadTrigger))]
    [ExportMetadata("Bundle", "Authorization")]
    [ExportMetadata("IsRavenExternalBundle", true)]
    public class AuthorizationReadTrigger : AbstractReadTrigger
    {
        public AuthorizationDecisions AuthorizationDecisions { get; set; }

        public override void Initialize()
        {
            AuthorizationDecisions = new AuthorizationDecisions(Database);	
        }

        public override ReadVetoResult AllowRead(string key, RavenJObject metadata, ReadOperation readOperation,
                                                 TransactionInformation transactionInformation)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                var user = (CurrentOperationContext.Headers.Value == null) ? null : CurrentOperationContext.Headers.Value.Value[Constants.Authorization.RavenAuthorizationUser];
                var operation = (CurrentOperationContext.Headers.Value == null)?null:CurrentOperationContext.Headers.Value.Value[Constants.Authorization.RavenAuthorizationOperation];
                if (string.IsNullOrEmpty(operation) || string.IsNullOrEmpty(user))
                    return ReadVetoResult.Allowed;

                var sw = new StringWriter();
                var isAllowed = AuthorizationDecisions.IsAllowed(user, operation, key, metadata, sw.WriteLine);
                if (isAllowed)
                    return ReadVetoResult.Allowed;
                return readOperation == ReadOperation.Query ?
                    ReadVetoResult.Ignore :
                    ReadVetoResult.Deny(sw.GetStringBuilder().ToString());
            }
        }
    }
}
