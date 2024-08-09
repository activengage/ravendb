using System;
using System.Linq;
using Raven35.Client;
using Raven35.Client.Embedded;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Nick : RavenTest
    {
        [Flags]
        public enum MyEnum
        {
            None = 0,
            First = 1,
            Second = 2
        }

        public class Entity
        {
            public string Id { set; get; }
            public string Name { set; get; }
            public MyEnum Status { set; get; }
        }

        public class MyIndex : AbstractIndexCreationTask<Entity, MyIndex.Result>
        {
            public class Result
            {
                public bool IsFirst;
                public bool IsSecond;
            }

            public MyIndex()
            {
                Map = entities => from entity in entities
                                  select new
                                  {
                                    IsFirst = (entity.Status & MyEnum.First) == MyEnum.First,
                                    IsSecond = (entity.Status & MyEnum.Second) == MyEnum.Second
                                  };
            }
        }

        [Fact]
        public void CanQueryUsingBitwiseOperations()
        {
            using (var store = new EmbeddableDocumentStore
            {
                RunInMemory = true,
                Conventions =
                    {
                        SaveEnumsAsIntegers = true
                    }
            })
            {
                store.Initialize();
                new MyIndex().Execute(store);

                using (var session = store.OpenSession())
                {
                    var entity = new Entity
                    {
                        Name = "name1",
                        Status = MyEnum.First | MyEnum.Second
                    };
                    session.Store(entity);
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var results = session.Query<MyIndex.Result, MyIndex>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                        .Where(x => x.IsSecond)
                        .As<Entity>()
                        .ToList();

                    Assert.Empty(store.SystemDatabase.Statistics.Errors);

                    Assert.NotEmpty(results);
                }
            }
        }
    }
}
