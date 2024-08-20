//-----------------------------------------------------------------------
// <copyright file="OverwriteIndexLocally.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class OverwriteIndexLocally : RavenTest
    {
        [Fact]
        public void CanOverwriteIndex()
        {
            using(var store = NewDocumentStore())
            {
                store.DatabaseCommands.PutIndex("test",
                                                new IndexDefinition
                                                {
                                                    Map = "from doc in docs select new { doc.Name }"
                                                }, overwrite:true);


                store.DatabaseCommands.PutIndex("test",
                                               new IndexDefinition
                                               {
                                                   Map = "from doc in docs select new { doc.Name }"
                                               }, overwrite: true);

                store.DatabaseCommands.PutIndex("test",
                                                new IndexDefinition
                                                {
                                                    Map = "from doc in docs select new { doc.Email }"
                                                }, overwrite: true);

                store.DatabaseCommands.PutIndex("test",
                                           new IndexDefinition
                                           {
                                               Map = "from doc in docs select new { doc.Email }"
                                           }, overwrite: true);
            }
        }
    }
}
