// -----------------------------------------------------------------------
//  <copyright file="QueryResultsStreaming.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class Companies_SortByName : AbstractIndexCreationTask<Company>
    {

        public Companies_SortByName()
        {
            Map = companies => from company in companies
                               select new { company.Name };

            Sort(c => c.Name, SortOptions.String);

#if !DNXCORE50
            Analyzers.Add(c => c.Name, typeof(Raven35.Database.Indexing.Collation.Cultures.PlCollationAnalyzer).ToString());
#else
            Analyzers.Add(c => c.Name, "Raven35.Database.Indexing.Collation.Cultures.PlCollationAnalyzer");
#endif
        }
    }
}
