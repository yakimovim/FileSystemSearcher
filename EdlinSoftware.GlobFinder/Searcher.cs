using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enumeration of all full paths of file system objects corresponding to <paramref name="globTemplate"/>. Never can be null.</returns>
        public Task<IEnumerable<string>> FindAsync(string globTemplate, CancellationToken cancellationToken)
        {
            var searchParameters = new SearchParameters(BaseDirectory, globTemplate);

            cancellationToken.ThrowIfCancellationRequested();

            return GetFileSystemObjectsAsync(searchParameters.BaseDirectory, searchParameters.SearchTemplateParts, cancellationToken);
        }

        private async Task<IEnumerable<string>> GetFileSystemObjectsAsync(string currentDirectory, string[] searchTemplateParts, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentPart = searchTemplateParts[0];

            if (searchTemplateParts.Length == 1)
                return await _fileSystem.GetFileSystemEntriesAsync(currentDirectory, currentPart, cancellationToken);

            if (currentPart == "**")
                return await GetFileSystemObjectsFromAllSubDirectoriesAsync(currentDirectory, searchTemplateParts.Skip(1).ToArray(), cancellationToken);

            var directories = await _fileSystem.GetDirectoriesAsync(currentDirectory, currentPart, cancellationToken);

            var results = new List<string>();
            foreach (var directory in directories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                results.AddRange(await GetFileSystemObjectsAsync(
                    directory,
                    searchTemplateParts.Skip(1).ToArray(),
                    cancellationToken));
            }
            return results;
        }

        private async Task<IEnumerable<string>> GetFileSystemObjectsFromAllSubDirectoriesAsync(string currentDirectory, string[] searchTemplateParts, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var results = new List<string>();

            var directories = await _fileSystem.GetDirectoriesAsync(currentDirectory, "*", cancellationToken);
            foreach (var directory in directories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                results.AddRange(await GetFileSystemObjectsFromAllSubDirectoriesAsync(
                    directory,
                    searchTemplateParts,
                    cancellationToken));
            }

            var currentPart = searchTemplateParts[0];

            if (searchTemplateParts.Length == 1)
            {
                results.AddRange(await _fileSystem.GetFileSystemEntriesAsync(currentDirectory, currentPart, cancellationToken));
            }
            else
            {
                directories = await _fileSystem.GetDirectoriesAsync(currentDirectory, currentPart, cancellationToken);

                foreach (var directory in directories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    results.AddRange(await GetFileSystemObjectsAsync(
                        directory,
                        searchTemplateParts.Skip(1).ToArray(),
                        cancellationToken));
                }
            }

            return results;
        }
    }

    public static class SearcherExtender
    {
        /// <summary>
        /// Returns enumeration of all full paths of file system objects corresponding to <paramref name="globTemplate"/>.
        /// </summary>
        /// <param name="searcher">Searcher.</param>
        /// <param name="globTemplate">Glob template.</param>
        /// <returns>Enumeration of all full paths of file system objects corresponding to <paramref name="globTemplate"/>. Never can be null.</returns>
        public static Task<IEnumerable<string>> FindAsync(this Searcher searcher, string globTemplate)
        {
            var source = new CancellationTokenSource();

            return searcher.FindAsync(globTemplate, source.Token);
        }
    }
}