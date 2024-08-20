// -----------------------------------------------------------------------
//  <copyright file="GlobalConfigurationTest.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Config.Retriever;
using Raven35.Tests.Common;

namespace Raven35.Tests.Issues.RavenDB_2712
{
    public class GlobalConfigurationTest : RavenTest
    {
        public GlobalConfigurationTest()
        {
            ConfigurationRetriever.EnableGlobalConfigurationOnce();
        } 
    }
}
