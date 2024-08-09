using Raven35.Abstractions.FileSystem;

namespace Raven35.Client.FileSystem.Listeners
{
    public interface IFilesDeleteListener
    {
        /// <summary>
        /// Invoked before the delete request is sent to the server.
        /// </summary>
        /// <param name="instance">The file to delete</param>
        bool BeforeDelete(FileHeader instance);

        /// <summary>
        /// Invoked after the delete operation was effective on the server.
        /// </summary>
        /// <param name="filename">The filename of the deleted file</param>
        void AfterDelete(string filename);
    }
}
