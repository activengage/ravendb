// -----------------------------------------------------------------------
//  <copyright file="QueryResultsStreaming.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class Companies_WithReferencedEmployees : AbstractIndexCreationTask<Company>
    {
        public class CompanyEmployees
        {
            public string Name { get; set; }
            public List<string> Employees { get; set; }
        }

        public Companies_WithReferencedEmployees()
        {
            Map = companies => from company in companies
                           select new
                           {
                               Name = company.Name,
                               EmployeesIds = company.EmployeesIds
                           };
        }
    }
}
