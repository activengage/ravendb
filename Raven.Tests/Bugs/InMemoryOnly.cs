//-----------------------------------------------------------------------
// <copyright file="InMemoryOnly.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using Raven35.Database.Extensions;
using Raven35.Database.Server;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class InMemoryOnly : RavenTest
    {
        [Fact]
        public void InMemoryDoesNotCreateDataDir()
        {
            IOExtensions.DeleteDirectory("Data");

            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8079);
            using (NewDocumentStore(runInMemory: true, port: 8079))
            {
                Assert.False(Directory.Exists("Data"));
            }
        }
    }
}
