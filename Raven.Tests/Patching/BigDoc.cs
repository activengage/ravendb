using System;
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Smuggler;
using Raven35.Database.Smuggler;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Patching
{
    public class BigDoc : RavenTest
    {
        [Fact]
        public void CanGetCorrectResult()
        {
            using (var store = NewDocumentStore())
            {
                using (var stream = typeof(BigDoc).Assembly.GetManifestResourceStream("Raven35.Tests.Patching.failingdump11.ravendbdump"))
                {
                    new DatabaseDataDumper(store.SystemDatabase).ImportData(new SmugglerImportOptions<RavenConnectionStringOptions> { FromStream = stream }).Wait(TimeSpan.FromSeconds(15));
                }

                using (var session = store.OpenSession())
                {
                    session.Advanced.DocumentQuery<object>("Raven/DocumentsByEntityName").WaitForNonStaleResults().ToList();

                    store.DatabaseCommands.UpdateByIndex("Raven/DocumentsByEntityName", new IndexQuery {Query = "Tag:Regions"},
                        new ScriptedPatchRequest
                        {
                            Script = @"this.Test = 'test';"
                        }
                        , new BulkOperationOptions {AllowStale = true, MaxOpsPerSec = null,StaleTimeout = null});
                }
            }
        }
    }
}
