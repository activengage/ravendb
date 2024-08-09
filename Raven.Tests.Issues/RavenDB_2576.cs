// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2576.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.IO;
using Raven35.Client.Embedded;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2576 : RavenTest
    {
        [Fact]
        public void EmbeddedStoreShouldCreateDatabasesInDataDirectory()
        {
            var path = NewDataPath();
            using (var store = new EmbeddableDocumentStore
            {
                DataDirectory = path
            }.Initialize())
            {
                Assert.True(Directory.Exists(path));
            }
        }
    }
}
