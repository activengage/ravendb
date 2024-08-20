//-----------------------------------------------------------------------
// <copyright file="CreatingIndexes.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Database.Config;
using Raven35.Database.Extensions;
using Raven35.Database.Server;
using Raven35.Server;
using Raven35.Tests.Common;

using Xunit;
using Raven35.Client.Extensions;

namespace Raven35.Tests.Bugs.MultiTenancy
{
    public class CreatingIndexes : RavenTest
    {
        [Fact]
        public void Multitenancy_Test()
        {
            using (GetNewServer(8079))
            using (var store = new DocumentStore
            {
                Url = "http://localhost:8079",
                DefaultDatabase = "Test"
            }.Initialize())
            {
                store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("Test");
                store.DatabaseCommands.PutIndex("TestIndex",
                                                new IndexDefinitionBuilder<Test, Test>("TestIndex")
                                                {
                                                    Map = movies => from movie in movies
                                                                    select new {movie.Name}
                                                });

                using (var session = store.OpenSession())
                {
                    session.Store(new Test {Name = "xxx"});

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var result = session.Query<Test>("TestIndex")
                        .Customize(x=>x.WaitForNonStaleResults())
                        .Where(x => x.Name == "xxx")
                        .FirstOrDefault();

                    Assert.NotNull(result);
                }
            }
        }

        public override void Dispose()
        {
            IOExtensions.DeleteDirectory("Data");
            IOExtensions.DeleteDirectory("Test");
            base.Dispose();
        }
    }
}
