using System;
using Raven35.Abstractions.FileSystem;

namespace Raven35.Database.FileSystem.Synchronization
{
    public class SynchronizationLock
    {
        public FileSystemInfo SourceFileSystem { get; set; }
        public DateTime FileLockedAt { get; set; }
    }
}
