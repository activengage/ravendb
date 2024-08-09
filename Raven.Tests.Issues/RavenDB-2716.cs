using System;
using System.CodeDom;
using System.Linq;
using Raven35.Abstractions;
using Raven35.Abstractions.Data;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Raven35.Tests.MailingList;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2716 : RavenTest
    {
        public class User
        {
            public String Name { get; set; }
        }

        public class Users_ByName : AbstractIndexCreationTask<User>
        {
            public Users_ByName()
            {
                Map = users => from user in users
                    select new {user.Name};
            }
        }

        [Fact]
        public void CanLimitOpsPerSecOnDelete()
        {
            using (var store = NewDocumentStore())
            {
                using (var bulkInsert = store.BulkInsert())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        bulkInsert.Store(new User
                        {
                            Name = "Users #" + i
                        },"users/"+i);
                    }
                }

                new Users_ByName().Execute(store);

                WaitForIndexing(store);

                var waits = 0;
                SystemTime.WaitCalled = ms => waits ++;

                var op = store.DatabaseCommands.DeleteByIndex("Users/ByName",
                                             new IndexQuery { Query = "Name:Users*" }
                                             , new BulkOperationOptions {AllowStale = false, MaxOpsPerSec = 1024,StaleTimeout = null});

                op.WaitForCompletion();

                Assert.True(waits > 3);
            }

        }

        [Fact]
        public void CanLimitOpsPerSecOnUpdate()
        {
            using (var store = NewDocumentStore())
            {
                using (var bulkInsert = store.BulkInsert())
                {
                    for (int i = 0; i < 100; i++)
                    {
                        bulkInsert.Store(new User
                        {
                            Name = "Users #" + i
                        },"users/"+i);
                    }
                }

                new Users_ByName().Execute(store);

                WaitForIndexing(store);

                var waits = 0;
                SystemTime.WaitCalled = ms => waits ++;

                var op = store.DatabaseCommands.UpdateByIndex("Users/ByName",
                                             new IndexQuery { Query = "Name:Users*" },
                                             new[]
                                                {
                                                    new PatchRequest
                                                        {
                                                            Type = PatchCommandType.Add,
                                                            Name = "Comments",
                                                            Value = "New automatic comment we added programmatically"
                                                        }
                                                }, new BulkOperationOptions {AllowStale = false, MaxOpsPerSec = 10,StaleTimeout = null});

                op.WaitForCompletion();

                Assert.True(waits > 3);
            }

        }
    }
}
