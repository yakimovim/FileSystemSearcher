using System;
using System.IO;
using System.Threading.Tasks;

namespace EdlinSoftware.FileSystemSearcher
{
    internal class DefaultFileSystem : IFileSystem
    {
        public string BaseDirectory => Environment.CurrentDirectory;

        public Task<string[]> GetFileSystemEntriesAsync(string path, string searchPattern)
        {
            return Task.Run(() => Directory.GetFileSystemEntries(path, searchPattern));
        }

        public Task<string[]> GetDirectoriesAsync(string path, string searchPattern)
        {
            return Task.Run(() => Directory.GetDirectories(path, searchPattern));
        }
    }
}