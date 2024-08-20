using System.Linq;
using Raven35.Client;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Raven35.Tests.Helpers;
using Xunit;

public class QueryCommaTest : RavenTest
{
    public class Employee
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }


    public class Employees_ByFirstName : AbstractIndexCreationTask<Employee>
    {
        public Employees_ByFirstName()
        {
            Map = employees => from employee in employees
                               select new
                               {
                                   employee.Id,
                                   employee.FirstName,
                               };
        }
    }


    [Fact]
    public void CommaInQueryTest()
    {
        using (var store = NewDocumentStore())
        {
            new Employees_ByFirstName().Execute(store);


            using (IDocumentSession session = store.OpenSession())
            {
                session
                    .Query<Employee, Employees_ByFirstName>()
                    .Search(x => x.FirstName, "foo , bar")
                    .ToList();
                // Lucene.Net.QueryParsers.ParseException: Could not parse:
                // ' FirstName:(foo , bar)' ---> Lucene.Net.QueryParsers.ParseException: Syntax error, unexpected COMMA
            }
        }
    }
}