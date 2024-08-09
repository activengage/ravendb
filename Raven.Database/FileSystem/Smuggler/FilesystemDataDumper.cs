// -----------------------------------------------------------------------
//  <copyright file="FilesystemDataDumper.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Smuggler;

namespace Raven35.Database.FileSystem.Smuggler
{
    public class FilesystemDataDumper : SmugglerFilesApiBase
    {
        public FilesystemDataDumper(RavenFileSystem fileSystem, SmugglerFilesOptions options = null) : base(options ?? new SmugglerFilesOptions())
        {
            Operations = new SmugglerEmbeddedFilesOperations(fileSystem);
        }

        public override Task Between(SmugglerBetweenOptions<FilesConnectionStringOptions> betweenOptions)
        {
            throw new NotSupportedException();
        }

        public Action<string> Progress
        {
            get
            {
                return ((SmugglerEmbeddedFilesOperations)Operations).Progress;
            }

            set
            {
                ((SmugglerEmbeddedFilesOperations)Operations).Progress = value;
            }
        }
    }
}
