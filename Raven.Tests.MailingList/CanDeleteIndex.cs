using System.Collections.Generic;
using System.Linq;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class CanDeleteIndex : RavenTest
    {
        private class AllDocs : AbstractIndexCreationTask<object>
        {
            public AllDocs() { Map = docs => from doc in docs select new { }; }
        }
            
        [Fact]
        public void WithNoErrors()
        {
            using(GetNewServer())
            using(var docStore = new DocumentStore
            {
                Url = "http://localhost:8079"
            }.Initialize())
            {
                new AllDocs().Execute(docStore);
                docStore.DatabaseCommands.DeleteIndex("AllDocs");
            }
        }
    }
}
