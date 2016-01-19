using System;
using System.IO;

namespace EdlinSoftware.FileSystemSearcher
{
    internal class DefaultFileSystem : IFileSystem
    {
        public string BaseDirectory => Environment.CurrentDirectory;

        public string[] GetFileSystemEntries(string path, string searchPattern)
        {
            return Directory.GetFileSystemEntries(path, searchPattern);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }
    }
}