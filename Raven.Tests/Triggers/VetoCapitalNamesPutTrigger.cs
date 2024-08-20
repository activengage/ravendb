//-----------------------------------------------------------------------
// <copyright file="VetoCapitalNamesPutTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using System.Linq;
using Raven35.Json.Linq;

namespace Raven35.Tests.Triggers
{
    public class VetoCapitalNamesPutTrigger : AbstractPutTrigger
    {
        public override VetoResult AllowPut(string key, RavenJObject document, RavenJObject metadata, TransactionInformation transactionInformation)
        {
            var name = document["name"];
            if(name != null && name.Value<string>().Any(char.IsUpper))
            {
                return VetoResult.Deny("Can't use upper case characters in the 'name' property");
            }
            return VetoResult.Allowed;
        }
    }
}
