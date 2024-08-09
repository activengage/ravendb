using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs.Indexing
{
    public class CreateIndexesOnRemoteServer : RavenTest
    {
        [Fact]
        public void CanCreateIndex()
        {
            using(GetNewServer())
            {
                using (var documentStore = new DocumentStore
                {
                    Url = "http://localhost:8079"
                }.Initialize())
                {
                    IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(typeof (SimpleIndex))), documentStore);
                    IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(typeof (SimpleIndex))), documentStore);
                }
            }
        }

        public class SimpleIndex : AbstractIndexCreationTask<User>
        {
            public SimpleIndex()
            {
                Map = users => from user in users
                               select new {user.Age};
            }
        }
    }
}
