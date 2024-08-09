using System.Linq;
using Raven35.Client.Indexes;

namespace Raven35.Tests.TestBase
{
    public class Animals_Search : AbstractMultiMapIndexCreationTask
    {
        public Animals_Search()
        {
            AddMapForAll<Animal>(animals => from animal in animals
                select new {animal.Name});
        }
    }
}
