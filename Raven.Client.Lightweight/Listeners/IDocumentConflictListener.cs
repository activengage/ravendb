using Raven35.Abstractions.Data;

namespace Raven35.Client.Listeners
{
    /// <summary>
    /// Hooks for users that allows you to handle document replication conflicts
    /// </summary>
    public interface IDocumentConflictListener
    {
        bool TryResolveConflict(string key, JsonDocument[] conflictedDocs, out JsonDocument resolvedDocument);
    }
}
