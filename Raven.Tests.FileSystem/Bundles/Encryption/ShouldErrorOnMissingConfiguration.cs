// -----------------------------------------------------------------------
//  <copyright file="FileSystemEncryptionTests.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.FileSystem;
using Raven35.Tests.Helpers;

using Xunit;

namespace Raven35.Tests.FileSystem.Bundles.Encryption
{
    public class ShouldErrorOnMissingConfiguration : RavenFilesTestBase
    {
        [Fact]
        public void ShouldThrow()
        {
            var client = NewAsyncClient();

            // secured setting nor specified
            var exception = Assert.Throws<AggregateException>(() => client.Admin.CreateFileSystemAsync(new FileSystemDocument()
            {
                Id = Constants.FileSystem.Prefix + "NewFS",
                Settings =
                {
                    {
                        Constants.ActiveBundles, "Encryption"
                    }
                },
                // SecuredSettings = new Dictionary<string, string>() - intentionally not saving them - should avoid NRE on server side 
            }).Wait());

            Assert.Equal("Failed to create 'NewFS' file system, because of invalid encryption configuration.", exception.InnerException.Message);

            // missing Constants.EncryptionKeySetting and Constants.AlgorithmTypeSetting
            exception = Assert.Throws<AggregateException>(() => client.Admin.CreateFileSystemAsync(new FileSystemDocument
            {
                Id = Constants.FileSystem.Prefix + "NewFS",
                Settings =
                {
                    {
                        Constants.ActiveBundles, "Encryption"
                    }
                },
                SecuredSettings = new Dictionary<string, string>()
            }).Wait());

            Assert.Equal("Failed to create 'NewFS' file system, because of invalid encryption configuration.", exception.InnerException.Message);

            // missing Constants.EncryptionKeySetting
            exception = Assert.Throws<AggregateException>(() => client.Admin.CreateFileSystemAsync(new FileSystemDocument()
            {
                Id = Constants.FileSystem.Prefix + "NewFS",
                Settings =
                {
                    {
                        Constants.ActiveBundles, "Encryption"
                    }
                },
                SecuredSettings = new Dictionary<string, string>()
                {
                    {Constants.EncryptionKeySetting, ""}
                }
            }).Wait());

            Assert.Equal("Failed to create 'NewFS' file system, because of invalid encryption configuration.", exception.InnerException.Message);

            // missing
            exception = Assert.Throws<AggregateException>(() => client.Admin.CreateFileSystemAsync(new FileSystemDocument()
            {
                Id = Constants.FileSystem.Prefix + "NewFS",
                Settings =
                {
                    {
                        Constants.ActiveBundles, "Encryption"
                    }
                },
                SecuredSettings = new Dictionary<string, string>()
                {
                    {Constants.AlgorithmTypeSetting, ""}
                }
            }).Wait());

            Assert.Equal("Failed to create 'NewFS' file system, because of invalid encryption configuration.", exception.InnerException.Message);
        }
    }
}
