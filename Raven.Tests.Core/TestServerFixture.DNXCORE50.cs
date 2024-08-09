﻿#if DNXCORE50
using Raven35.Abstractions.Connection;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raven35.Tests.Core
{
    public class TestServerFixture : IDisposable
    {
        public const int Port = 8070;
        public const string ServerName = "Raven35.Tests.Core.Server";

        public IDocumentStore DocumentStore { get; private set; }

        public static string ServerUrl { get; private set; }

        private Process process;

        public TestServerFixture()
        {
            StartProcess();

            CreateServerAsync().Wait();
            CreateStore();
        }

        private void CreateStore()
        {
            var store = new DocumentStore { Url = ServerUrl };
            store.Initialize();

            DocumentStore = store;
        }

        private static async Task CreateServerAsync()
        {
            var httpClient = new HttpClient();
            var serverConfiguration = new ServerConfiguration
            {
                DefaultStorageTypeName = "voron",
                Port = Port,
                RunInMemory = true,
                Settings =
                {
                    { "Raven35.ServerName", ServerName }
                }
            };

            var response = await httpClient
                .PutAsync("http://localhost:8585/servers", new JsonContent(RavenJObject.FromObject(serverConfiguration)))
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode == false)
                throw new InvalidOperationException("Failed to start server.");

            using (var stream = await response.GetResponseStreamWithHttpDecompression().ConfigureAwait(false))
            {
                var data = RavenJToken.TryLoad(stream);
                if (data == null)
                    throw new InvalidOperationException("Failed to retrieve server url.");

                ServerUrl = data.Value<string>("ServerUrl");
            }
        }

        private void StartProcess()
        {
            KillServerRunner();

            var path = GetServerRunnerPath();
            var startInfo = new ProcessStartInfo(path)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            process = Process.Start(startInfo);
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing the DocumentStore");

            if (DocumentStore != null)
                DocumentStore.Dispose();

            Console.WriteLine("Disposed the DocumentStore");

            try
            {
                Console.WriteLine("Killing the process");
                process?.Kill();
                Console.WriteLine("Killed the process");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not kill the process. Exception: " + e);
            }

            KillServerRunner();
        }

        private static void KillServerRunner()
        {
            Console.WriteLine("Killing the ServerRunner");

            var processes = Process.GetProcessesByName("Raven35.Tests.Server.Runner.exe");
            foreach (var p in processes)
            {
                try
                {
                    Console.WriteLine("Killing the server runner process");
                    p.Kill();
                    Console.WriteLine("Killed the server runner process");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not kill the server runner process. Exception: " + e);
                }
            }

            Console.WriteLine("Killed the ServerRunner");
        }

        private static string GetServerRunnerPath()
        {
#if DEBUG
            var path = "Raven35.Tests.Server.Runner/bin/Debug/Raven35.Tests.Server.Runner.exe";
#else
            var path = "Raven35.Tests.Server.Runner/bin/Release/Raven35.Tests.Server.Runner.exe";
#endif

            var tries = 10;
            while (tries > 0)
            {
                path = Path.Combine("../", path);
                var fullPath = Path.GetFullPath(path);

                if (File.Exists(fullPath))
                {
                    path = fullPath;
                    break;
                }

                tries--;
            }

            if (File.Exists(path) == false)
                throw new InvalidOperationException(string.Format("Could not locate 'Raven35.Tests.Server.Runner' in '{0}'.", path));

            return path;
        }

        private class ServerConfiguration
        {
            public ServerConfiguration()
            {
                Settings = new Dictionary<string, string>();
            }

            public int Port { get; set; }

            public bool RunInMemory { get; set; }

            public string DefaultStorageTypeName { get; set; }

            public bool UseCommercialLicense { get; set; }

            public string ApiKeyName { get; set; }

            public string ApiKeySecret { get; set; }

            public IDictionary<string, string> Settings { get; set; }

            public bool HasApiKey { get { return !string.IsNullOrEmpty(ApiKeyName) && !string.IsNullOrEmpty(ApiKeySecret); } }
        }
    }
}
#endif