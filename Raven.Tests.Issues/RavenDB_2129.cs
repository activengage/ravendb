// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2129.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

using Raven35.Abstractions;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Connection;
using Raven35.Client.Document;
using Raven35.Client.Extensions;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto;

using Xunit;
using Raven35.Client.Connection.Async;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2129 : ReplicationBase
    {
        [Fact(Skip = "Flaky test")]
        public async Task ShouldReadFromSecondaryServerWhenPrimaryIsDown()
        {
            using (var store1 = CreateStore(configureStore: store => store.Conventions.FailoverBehavior = FailoverBehavior.AllowReadsFromSecondaries))
            using (var store2 = CreateStore())
            {
                await store1.AsyncDatabaseCommands.GlobalAdmin.EnsureDatabaseExistsAsync("SomeDB");
                await store2.AsyncDatabaseCommands.GlobalAdmin.EnsureDatabaseExistsAsync("SomeDB");

                RunReplication(store1, store2, db: "SomeDB");

                SystemTime.UtcDateTime = () => DateTime.UtcNow.AddMinutes(10); // this will force replication information update when session is opened
                
                await store1.GetReplicationInformerForDatabase("SomeDB")
                    .UpdateReplicationInformationIfNeededAsync((AsyncServerClient)store1.AsyncDatabaseCommands.ForDatabase("SomeDB"),force:true);                    

                using (var session = store1.OpenAsyncSession("SomeDB"))
                {
                    await session.StoreAsync(new Person { Name = "Person1" });
                    await session.SaveChangesAsync();
                }
                

                WaitForReplication(store2, "people/1", db: "SomeDB");

                StopDatabase(0);

                using (var session = store1.OpenAsyncSession("SomeDB"))
                {
                    var person = await session.LoadAsync<Person>("people/1");
                    Assert.Equal("Person1", person.Name);
                }
            }
        } 
    }
}
