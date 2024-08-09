// -----------------------------------------------------------------------
//  <copyright file="UpperCaseNamesTrigger.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Tests.Common.Triggers
{
    public class UpperCaseNamesTrigger : AbstractReadTrigger
    {
        public override void OnRead(string key, RavenJObject document, RavenJObject metadata, ReadOperation operation, TransactionInformation transactionInformation)
        {
            var name = document["name"];
            if (name != null)
            {
                document["name"] = new RavenJValue(name.Value<string>().ToUpper());
            }
        }
    }
}
