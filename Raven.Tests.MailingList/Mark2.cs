using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Mark2 : RavenTest
    {
        [Fact]
        public void ShouldNotGetErrors()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test", new IndexDefinition
                {
                    Map = @"from brief in docs.TestCases
 select new {
 _tWarnings_AccessoryWarnings_Value = brief.Warnings.AccessoryWarnings.Select(y=>y.Value)
 }"
                });

                store.DatabaseCommands.Put("TestCases/TST00001", null,
                                           RavenJObject.Parse(
                                            @"{
 ""Warnings"": {
   ""AccessoryWarnings"": [
     {
       ""Value"": ""whatever"",
       ""Id"": 123
     },
     {
       ""Value"": ""dsfsdfsd sfsd sd"",
       ""Id"": 1234
     }
   ]
 }
}"),
                                           new RavenJObject {{Constants.RavenEntityName, "TestCases"}});

                store.DatabaseCommands.Put("TestCases/TST00002", null,
                                           RavenJObject.Parse(
                                            @"{
 ""Warnings"": {
   ""AccessoryWarnings"": []
 }
}"),
                                           new RavenJObject { { Constants.RavenEntityName, "TestCases" } });

                WaitForIndexing(store);

                Assert.Empty(store.SystemDatabase.Statistics.Errors);
            }
        }
    }
}
