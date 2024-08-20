// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2586.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.Smuggler;
using Raven35.Client.Extensions;
using Raven35.Smuggler;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2586 : RavenTest
    {
        [Fact]
        public void SmugglerBetweenOperationShouldNotCreateDatabases()
        {
            using (var store = NewRemoteDocumentStore())
            {
                var smugglerApi = new SmugglerDatabaseApi();

                var options = new SmugglerBetweenOptions<RavenConnectionStringOptions>
                              {
                                    From = new RavenConnectionStringOptions
                                    {
                                        Url = store.Url, 
                                        DefaultDatabase = "DB1"
                                    },
                                    To = new RavenConnectionStringOptions
                                    {
                                        Url = store.Url, 
                                        DefaultDatabase = "DB2"
                                    }
                              };

                var aggregateException = Assert.Throws<AggregateException>(() => smugglerApi.Between(options).Wait());
                var exception = aggregateException.ExtractSingleInnerException();
                Assert.True(exception.Message.StartsWith("Smuggler does not support database creation (database 'DB1' on server"));

                store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("DB1");

                aggregateException = Assert.Throws<AggregateException>(() => smugglerApi.Between(options).Wait());
                exception = aggregateException.ExtractSingleInnerException();
                Assert.True(exception.Message.StartsWith("Smuggler does not support database creation (database 'DB2' on server"));
            }
        }
    }
}
