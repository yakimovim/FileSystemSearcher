namespace EdlinSoftware.FileSystemSearcher
{
    /// <summary>
    /// Represents file system.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Returns base directory for searcher.
        /// </summary>
        string BaseDirectory { get; }

        /// <summary>
        /// Returns all file system entries in the current <paramref name="path"/> corresponding to <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="path">Base path.</param>
        /// <param name="searchPattern">Search pattern.</param>
        /// <returns>All file system entries in the current <paramref name="path"/> corresponding to <paramref name="searchPattern"/>.</returns>
        string[] GetFileSystemEntries(string path, string searchPattern);

        /// <summary>
        /// Returns all directories in the current <paramref name="path"/> corresponding to <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="path">Base path.</param>
        /// <param name="searchPattern">Search pattern.</param>
        /// <returns>All directories in the current <paramref name="path"/> corresponding to <paramref name="searchPattern"/>.</returns>
        string[] GetDirectories(string path, string searchPattern);
    }
}