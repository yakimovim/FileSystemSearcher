using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EdlinSoftware.FileSystemSearcher.Tests
{
    public class FakeFileSystem : IFileSystem
    {
        public IDictionary<string, IList<string>> Directories { get; set; } = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<string, IList<string>> Files { get; set; } = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);

        public string BaseDirectory { get; set; } = @"c:\";

        public Task<string[]> GetFileSystemEntriesAsync(string path, string searchPattern, CancellationToken cancellationToken)
        {
            return Task.FromResult(GetEntries(GetFileSystemObjects(Directories, path)
                .Concat(GetFileSystemObjects(Files, path)),
                path,
                searchPattern));
        }

        public Task<string[]> GetDirectoriesAsync(string path, string searchPattern, CancellationToken cancellationToken)
        {
            return Task.FromResult(GetEntries(GetFileSystemObjects(Directories, path),
                path,
                searchPattern));
        }

        private IEnumerable<string> GetFileSystemObjects(IDictionary<string, IList<string>> storage, string path)
        {
            path = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var baseDirectory = BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (path.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
                path = path.Substring(baseDirectory.Length);

            path = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (!storage.ContainsKey(path))
                return new string[0];

            return storage[path];
        }

        private string[] GetEntries(IEnumerable<string> listOfEntries, string path, string searchPattern)
        {
            var regexPattern = searchPattern
                .Replace(".", @"\.")
                .Replace("*", ".*")
                .Replace("?", ".");

            return listOfEntries
                .Where(e => Regex.IsMatch(e, regexPattern))
                .Select(e => Path.Combine(path, e))
                .ToArray();
        }

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);
    }

    public class DelayedFileSystem : IFileSystem
    {
        private readonly IFileSystem _fileSystem;
        private readonly TimeSpan _delay;

        public DelayedFileSystem(IFileSystem fileSystem, TimeSpan delay)
        {
            _fileSystem = fileSystem;
            _delay = delay;
        }

        public string BaseDirectory => _fileSystem.BaseDirectory;

        public async Task<string[]> GetFileSystemEntriesAsync(string path, string searchPattern, CancellationToken cancellationToken)
        {
            await Task.Delay(_delay, cancellationToken);

            return await _fileSystem.GetFileSystemEntriesAsync(path, searchPattern, cancellationToken);
        }

        public async Task<string[]> GetDirectoriesAsync(string path, string searchPattern, CancellationToken cancellationToken)
        {
            await Task.Delay(_delay, cancellationToken);

            return await _fileSystem.GetDirectoriesAsync(path, searchPattern, cancellationToken);
        }
    }
}