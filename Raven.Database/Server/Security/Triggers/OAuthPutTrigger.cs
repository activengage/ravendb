// -----------------------------------------------------------------------
//  <copyright file="OAuthPutTrigger.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Plugins;

namespace Raven35.Database.Server.Security.Triggers
{
    public class OAuthPutTrigger : AbstractPutTrigger
    {
        public override VetoResult AllowPut(string key, Raven35.Json.Linq.RavenJObject document, Raven35.Json.Linq.RavenJObject metadata, Raven35.Abstractions.Data.TransactionInformation transactionInformation)
        {
            if (key != null && key.StartsWith("Raven/ApiKeys/") && Authentication.IsEnabled == false)
                return VetoResult.Deny("Cannot setup OAuth Authentication without a valid commercial license.");

            return VetoResult.Allowed;
        }
    }
}
