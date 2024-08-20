using Raven35.Abstractions.FileSystem;

namespace Raven35.Database.FileSystem.Synchronization.Conflictuality
{
    public class ConflictResolution
    {
        public ConflictResolutionStrategy Strategy { get; set; }
        public long Version { get; set; }
        public string RemoteServerId { get; set; }
    }
}
