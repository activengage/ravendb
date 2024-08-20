#if !DNXCORE50
// -----------------------------------------------------------------------
//  <copyright file="CoreTestServer.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.IO;
using Raven35.Database.Config;
using Raven35.Database.Extensions;
using Raven35.Database.Server;
using Raven35.Server;
using Raven35.Tests.Common.Util;
using Raven35.Tests.Helpers.Util;

namespace Raven35.Tests.Core
{
    public class TestServerFixture : IDisposable
    {
        public const int Port = 8079;
        public const string ServerName = "Raven35.Tests.Core.Server";

        public TestServerFixture()
        {
            var configuration = new RavenConfiguration();

            ConfigurationHelper.ApplySettingsToConfiguration(configuration);

            configuration.Port = Port;
            configuration.ServerName = ServerName;
            configuration.RunInMemory = configuration.DefaultStorageTypeName == InMemoryRavenConfiguration.VoronTypeName;
            configuration.DataDirectory = Path.Combine(configuration.DataDirectory, "Tests");
            configuration.MaxSecondsForTaskToWaitForDatabaseToLoad = 10;
            configuration.Storage.Voron.AllowOn32Bits = true;

            IOExtensions.DeleteDirectory(configuration.DataDirectory);

            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(Port);

            Server = new RavenDbServer(configuration)
            {
                UseEmbeddedHttpServer = true,
                RunInMemory = true
            }.Initialize();
        }

        public RavenDbServer Server { get; private set; }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}

#endif