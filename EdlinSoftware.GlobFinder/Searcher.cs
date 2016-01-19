using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<string> Find(string globTemplate)
        {
            var searchParameters = new SearchParameters(BaseDirectory, globTemplate);

            return GetFileSystemObjects(searchParameters.BaseDirectory, searchParameters.SearchTemplateParts);
        }

        private IEnumerable<string> GetFileSystemObjects(string currentDirectory, string[] searchTemplateParts)
        {
            var currentPart = searchTemplateParts[0];

            if (searchTemplateParts.Length == 1)
                return _fileSystem.GetFileSystemEntries(currentDirectory, currentPart);

            if (currentPart == "**")
                return GetFileSystemObjectsFromAllSubDirectories(currentDirectory, searchTemplateParts.Skip(1).ToArray());

            var directories = _fileSystem.GetDirectories(currentDirectory, currentPart);

            var results = new List<string>();
            foreach (var directory in directories)
            {
                results.AddRange(GetFileSystemObjects(
                    directory,
                    searchTemplateParts.Skip(1).ToArray()));
            }
            return results;
        }

        private IEnumerable<string> GetFileSystemObjectsFromAllSubDirectories(string currentDirectory, string[] searchTemplateParts)
        {
            var results = new List<string>();

            var directories = _fileSystem.GetDirectories(currentDirectory, "*");
            foreach (var directory in directories)
            {
                results.AddRange(GetFileSystemObjectsFromAllSubDirectories(
                    directory,
                    searchTemplateParts));
            }

            var currentPart = searchTemplateParts[0];

            if (searchTemplateParts.Length == 1)
            {
                results.AddRange(_fileSystem.GetFileSystemEntries(currentDirectory, currentPart));
            }
            else
            {
                directories = _fileSystem.GetDirectories(currentDirectory, currentPart);

                foreach (var directory in directories)
                {
                    results.AddRange(GetFileSystemObjects(
                        directory,
                        searchTemplateParts.Skip(1).ToArray()));
                }
            }

            return results;
        }
    }
}