//-----------------------------------------------------------------------
// <copyright file="WhenUsingMultipleUnshardedServers.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Document
{
    public class WhenUsingMultipleUnshardedServers : RavenTest
    {
        private readonly int port1;
        private readonly int port2;

        public WhenUsingMultipleUnshardedServers()
        {
            port1 = 8079;
            port2 = 8081;
        }

        [Fact]
        public void CanInsertIntoTwoServersRunningSimultaneouslyWithoutSharding()
        {
            using (var server1 = GetNewServer(port1))
            using (var server2 = GetNewServer(port2))
            {
                foreach (var port in new[] { port1, port2 })
                {
                    using (var documentStore = new DocumentStore { Url = "http://localhost:"+ port }.Initialize())
                    using (var session = documentStore.OpenSession())
                    {
                        var entity = new Company { Name = "Company" };
                        session.Store(entity);
                        session.SaveChanges();
                        Assert.NotEqual(Guid.Empty.ToString(), entity.Id);
                    }
                }
            }
        }
    }
}
