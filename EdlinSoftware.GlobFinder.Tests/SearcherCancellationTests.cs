using System;
using System.Threading;
using Xunit;

namespace EdlinSoftware.FileSystemSearcher.Tests
{
    public class SearcherCancellationTests : SearcherTestBase
    {
        private const string BaseDirectory = @"c:\Some\Absolute\Path\";

        private readonly Searcher _searcher;

        public SearcherCancellationTests()
        {
            FileSystem.BaseDirectory = BaseDirectory;
            _searcher = new Searcher(new DelayedFileSystem(FileSystem, TimeSpan.FromMilliseconds(50))) { BaseDirectory = @"c:\Another\Path\" };
        }

        [Fact]
        public async void Find_AllSubDirectories_ExistingFiles()
        {
            var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

            await _searcher.FindAsync(BaseDirectory + @"**/*.dat", source.Token)
                .ContinueWith(searchTask =>
                {
                    Assert.True(searchTask.IsCanceled, "Search task should be cancelled.");
                }, new CancellationTokenSource().Token);
        }
    }
}