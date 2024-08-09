using System.Linq;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class EnumAsInts : RavenTest
    {
        public enum Flags
        {
            One = 1,
            Two = 2,
            Four = 4
        }
        public class Item
        {
            public Flags Flags { get; set; }
        }
        public class Index : AbstractIndexCreationTask<Item>
        {
            public Index()
            {
                Map = items => from item in items
                               where (item.Flags & Flags.Four) == Flags.Four
                               select new {item.Flags};
            }
        }

        [Fact]
        public void CanWork()
        {
            using(var store = NewDocumentStore())
            {
                store.Conventions.SaveEnumsAsIntegers = true;
                new Index().Execute(store);
            }
        }
    }
}
