//-----------------------------------------------------------------------
// <copyright file="AuthorizationTest.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using System.ComponentModel.Composition.Hosting;
using System.Web;

using Raven35.Bundles.Authorization;
using Raven35.Client.Document;
using Raven35.Database;
using Raven35.Database.Server.WebApi;
using Raven35.Server;
using Raven35.Tests.Common;

namespace Raven35.Tests.Bundles.Authorization
{
    public abstract class AuthorizationTest : RavenTest
    {
        protected const string UserId = "Authorization/Users/Ayende";
        protected readonly DocumentStore store;
        protected readonly RavenDbServer server;

        protected readonly string DatabaseName = Raven35.Abstractions.Data.Constants.SystemDatabase;
        
        protected AuthorizationTest()
        {
            RouteCacher.ClearCache();

            server = GetNewServer(activeBundles: "Authorization", configureConfig: 
                configuration => configuration.Catalog.Catalogs.Add(new AssemblyCatalog(typeof(AuthorizationDecisions).Assembly)));
            store = NewRemoteDocumentStore(ravenDbServer: server,  databaseName: DatabaseName);
            
            foreach (DictionaryEntry de in HttpRuntime.Cache)
            {
                HttpRuntime.Cache.Remove((string)de.Key);
            }
        }

        protected DocumentDatabase Database
        {
            get
            {
                return server.Server.GetDatabaseInternal(DatabaseName).Result;
            }
        }
    }
}
