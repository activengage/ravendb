//-----------------------------------------------------------------------
// <copyright file="SuggestionQueryExtensions.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Abstractions.MEF;
using Raven35.Database.Plugins;

namespace Raven35.Database.Queries
{
    public static class MoreLikeThisQueryExtensions
    {
        public static MoreLikeThisQueryResult ExecuteMoreLikeThisQuery(this DocumentDatabase self, MoreLikeThisQuery query, TransactionInformation transactionInformation, int pageSize, OrderedPartCollection<AbstractIndexQueryTrigger> databaseIndexQueryTriggers)
        {
            return new MoreLikeThisQueryRunner(self).ExecuteMoreLikeThisQuery(query, transactionInformation, pageSize, databaseIndexQueryTriggers);
        }
    }
}
