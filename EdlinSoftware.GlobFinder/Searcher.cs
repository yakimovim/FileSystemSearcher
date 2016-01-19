using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdlinSoftware.FileSystemSearcher
{
    /// <summary>
    /// Represents searcher of file system objects by extended template.
    /// </summary>
    public class Searcher
    {
        private readonly IFileSystem _fileSystem;

        public Searcher(IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new DefaultFileSystem();
            BaseDirectory = _fileSystem.BaseDirectory;
        }

        /// <summary>
        /// Gets or sets base directory relative to which search is processed.
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Returns enumeration of all full paths of file system objects corresponding to <paramref name="globTemplate"/>.
        /// </summary>
        /// <param name="globTemplate">Glob template.</param>
        /// <returns>Enumeration of all full paths of file system objects corresponding to <paramref name="globTemplate"/>. Never can be null.</returns>
        public Task<IEnumerable<string>> FindAsync(string globTemplate)
        {
            var searchParameters = new SearchParameters(BaseDirectory, globTemplate);

            return GetFileSystemObjectsAsync(searchParameters.BaseDirectory, searchParameters.SearchTemplateParts);
        }

        private async Task<IEnumerable<string>> GetFileSystemObjectsAsync(string currentDirectory, string[] searchTemplateParts)
        {
            var currentPart = searchTemplateParts[0];

            if (searchTemplateParts.Length == 1)
                return await _fileSystem.GetFileSystemEntriesAsync(currentDirectory, currentPart);

            if (currentPart == "**")
                return await GetFileSystemObjectsFromAllSubDirectoriesAsync(currentDirectory, searchTemplateParts.Skip(1).ToArray());

            var directories = await _fileSystem.GetDirectoriesAsync(currentDirectory, currentPart);

            var results = new List<string>();
            foreach (var directory in directories)
            {
                results.AddRange(await GetFileSystemObjectsAsync(
                    directory,
                    searchTemplateParts.Skip(1).ToArray()));
            }
            return results;
        }

        private async Task<IEnumerable<string>> GetFileSystemObjectsFromAllSubDirectoriesAsync(string currentDirectory, string[] searchTemplateParts)
        {
            var results = new List<string>();

            var directories = await _fileSystem.GetDirectoriesAsync(currentDirectory, "*");
            foreach (var directory in directories)
            {
                results.AddRange(await GetFileSystemObjectsFromAllSubDirectoriesAsync(
                    directory,
                    searchTemplateParts));
            }

            var currentPart = searchTemplateParts[0];

            if (searchTemplateParts.Length == 1)
            {
                results.AddRange(await _fileSystem.GetFileSystemEntriesAsync(currentDirectory, currentPart));
            }
            else
            {
                directories = await _fileSystem.GetDirectoriesAsync(currentDirectory, currentPart);

                foreach (var directory in directories)
                {
                    results.AddRange(await GetFileSystemObjectsAsync(
                        directory,
                        searchTemplateParts.Skip(1).ToArray()));
                }
            }

            return results;
        }
    }
}