//-----------------------------------------------------------------------
// <copyright file="AuthorizationPutTrigger.cs" company="Hibernating Rhinos LTD">
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
    [InheritedExport(typeof(AbstractPutTrigger))]
    [ExportMetadata("Bundle", "Authorization")]
    [ExportMetadata("IsRavenExternalBundle", true)]
    public class AuthorizationPutTrigger : AbstractPutTrigger
    {
        public AuthorizationDecisions AuthorizationDecisions { get; set; }

        public override void Initialize()
        {
            AuthorizationDecisions = new AuthorizationDecisions(Database);
        }

        public override VetoResult AllowPut(string key, RavenJObject document, RavenJObject metadata, TransactionInformation transactionInformation)
        {
            using (Database.DisableAllTriggersForCurrentThread())
            {
                var user = (CurrentOperationContext.Headers.Value == null) ? null : CurrentOperationContext.Headers.Value.Value[Constants.Authorization.RavenAuthorizationUser];
                var operation = (CurrentOperationContext.Headers.Value == null) ? null : CurrentOperationContext.Headers.Value.Value[Constants.Authorization.RavenAuthorizationOperation];
                if (string.IsNullOrEmpty(operation) || string.IsNullOrEmpty(user))
                    return VetoResult.Allowed;

                var previousDocument = Database.Documents.Get(key, transactionInformation);
                var metadataForAuthorization = previousDocument != null ? previousDocument.Metadata : metadata;

                var sw = new StringWriter();
                var isAllowed = AuthorizationDecisions.IsAllowed(user, operation, key, metadataForAuthorization, sw.WriteLine);
                return isAllowed ?
                    VetoResult.Allowed :
                    VetoResult.Deny(sw.GetStringBuilder().ToString());
            }
        }
    }
}
