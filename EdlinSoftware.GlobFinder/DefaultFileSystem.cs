using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EdlinSoftware.FileSystemSearcher
{
    internal class DefaultFileSystem : IFileSystem
    {
        public string BaseDirectory => Environment.CurrentDirectory;

        public Task<string[]> GetFileSystemEntriesAsync(string path, string searchPattern, CancellationToken cancellationToken)
        {
            return Task.Run(() => Directory.GetFileSystemEntries(path, searchPattern), cancellationToken);
        }

        public Task<string[]> GetDirectoriesAsync(string path, string searchPattern, CancellationToken cancellationToken)
        {
            return Task.Run(() => Directory.GetDirectories(path, searchPattern), cancellationToken);
        }
    }
}