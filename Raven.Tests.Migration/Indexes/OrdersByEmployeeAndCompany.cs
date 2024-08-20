// -----------------------------------------------------------------------
//  <copyright file="OrdersByEmployeeAndCompany.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;

namespace Raven35.Tests.Migration.Indexes
{
    public class OrdersByEmployeeAndCompany : AbstractIndexCreationTask
    {
        public override string IndexName
        {
            get
            {
                return "Orders/ByEmployeeAndCompany";
            }
        }
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
            {
                Map = @"from order in docs.Orders
let employee = LoadDocument(order.Employee)
let company = LoadDocument(order.Company)
select new {
    LastName = employee.LastName,
    FirstName = employee.FirstName,
    Company = company.Name
}"
            };
        }
    }
}
