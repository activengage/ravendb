// -----------------------------------------------------------------------
//  <copyright file="UsersWithCustomDataAndInclude.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Transformers
{
    public class CompanyEmployeesTransformer : AbstractTransformerCreationTask<Company>
    {
        public CompanyEmployeesTransformer()
        {
            TransformResults = companies => from company in companies
                                                      select new
                                                      {
                                                          Name = company.Name,
                                                          Employees = company.EmployeesIds.Select(x => LoadDocument<Employee>(x).LastName)
                                                      };
        }
    }
}
