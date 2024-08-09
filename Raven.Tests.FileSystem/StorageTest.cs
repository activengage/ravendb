using System;
using System.Collections.Specialized;
using Raven35.Abstractions.MEF;
using Raven35.Database.Config;
using Raven35.Database.Extensions;
using Raven35.Database.FileSystem.Infrastructure;
using Raven35.Database.FileSystem.Plugins;
using Raven35.Database.FileSystem.Storage.Esent;
using Raven35.Database.Plugins;

namespace Raven35.Tests.FileSystem
{
    public class StorageTest : IDisposable
    {
        protected readonly TransactionalStorage transactionalStorage;

        public StorageTest()
        {
            var configuration = new InMemoryRavenConfiguration
                                {
                                    Settings = new NameValueCollection(), 
                                    FileSystem =
                                    {
                                        DataDirectory = "test"
                                    }
                                };

            IOExtensions.DeleteDirectory("test");
            transactionalStorage = new TransactionalStorage(configuration);
            transactionalStorage.Initialize(new UuidGenerator(), new OrderedPartCollection<AbstractFileCodec>());
        }

        public void Dispose()
        {
            transactionalStorage.Dispose();

            IOExtensions.DeleteDirectory("test");
        }
    }
}
