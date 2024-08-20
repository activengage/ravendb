//-----------------------------------------------------------------------
// <copyright file="CascadeDeleteTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Database;
using Raven35.Database.Plugins;

namespace Raven35.Tests.Triggers
{
    public class CascadeDeleteTrigger : AbstractDeleteTrigger 
    {
        public override VetoResult AllowDelete(string key, TransactionInformation transactionInformation)
        {
            return VetoResult.Allowed;
        }

        public override void OnDelete(string key, TransactionInformation transactionInformation)
        {
            var document = Database.Documents.Get(key, null);
            if (document == null)
                return;
            var value = document.Metadata.Value<string>("Cascade-Delete");
            if(value != null)
            {
                Database.Documents.Delete(value, null, null);
            }
        }

        public override void AfterCommit(string key)
        {
        }
    }
}
