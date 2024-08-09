// -----------------------------------------------------------------------
//  <copyright file="HeadAsync.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Threading.Tasks;

using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class HeadAsync : RavenTest
    {
         [Fact]
         public async Task ShouldReturnNull()
         {
             using (var store = NewRemoteDocumentStore())
             {
                 Assert.Null(await store.AsyncDatabaseCommands.HeadAsync("test"));
             }
         }
    }
}
