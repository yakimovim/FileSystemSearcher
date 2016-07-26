using System.IO;
using System.Linq;
using Xunit;

namespace EdlinSoftware.FileSystemSearcher.Tests
{
    public class SearcherTestsForNetworkPaths : SearcherTestBase
    {
        private const string BaseDirectory = @"\\SOME-COMPUTER\TestData\";

        private readonly Searcher _searcher;

        public SearcherTestsForNetworkPaths()
        {
            FileSystem.BaseDirectory = BaseDirectory;
            _searcher = new Searcher(FileSystem) {BaseDirectory = @"c:\Some\Path\"};
        }

        [Fact]
        public async void Find_DirectFileName_AbsentFile()
        {
            var result = await _searcher.FindAsync(BaseDirectory + "System.dll");

            Assert.False(result.Any(), "Absent file should not be found");
        }

        [Fact]
        public async void Find_DirectFileName_ExistingFile()
        {
            var result = (await _searcher.FindAsync(BaseDirectory + "test.dat")).ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(BaseDirectory, "test.dat"), result[0]);
        }

        [Fact]
        public async void Find_TemplateFileName_AbsentFile()
        {
            var result = await _searcher.FindAsync(BaseDirectory + "a*t.dat");

            Assert.False(result.Any(), "Absent files should not be found");
        }

        [Fact]
        public async void Find_TemplateFileName_ExistingFile()
        {
            var result = (await _searcher.FindAsync(BaseDirectory + "t*t.dat")).ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(BaseDirectory, "test.dat"), result[0]);
        }

        [Fact]
        public async void Find_DirectoryInTemplate_AbsentFile()
        {
            var result = await _searcher.FindAsync(BaseDirectory + @"bin\System.dll");

            Assert.False(result.Any(), "Absent file should not be found");
        }

        [Fact]
        public async void Find_DirectoryInTemplate_ExistingFile()
        {
            var result = (await _searcher.FindAsync(BaseDirectory + @"bin\result.dat")).ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(BaseDirectory, "bin", "result.dat"), result[0]);
        }

        [Fact]
        public async void Find_WildCardInDirectoryInTemplate_ExistingFile()
        {
            var result = (await _searcher.FindAsync(BaseDirectory + @"b?n\result.dat")).ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(BaseDirectory, "bin", "result.dat"), result[0]);
        }

        [Fact]
        public async void Find_AllDirectories_ExistingFiles()
        {
            var result = (await _searcher.FindAsync(BaseDirectory + @"*/*.dat")).ToArray();

            Assert.Equal(3, result.Length);

            Assert.Contains(Path.Combine(BaseDirectory, "bin", "result.dat"), result);
            Assert.Contains(Path.Combine(BaseDirectory, "obj", "intermediate.dat"), result);
            Assert.Contains(Path.Combine(BaseDirectory, "some.info", "info.dat"), result);
        }

        [Fact]
        public async void Find_AllSubDirectories_ExistingFiles()
        {
            var result = (await _searcher.FindAsync(BaseDirectory + @"**/*.dat")).ToArray();

            Assert.Equal(5, result.Length);

            Assert.Contains(Path.Combine(BaseDirectory, "test.dat"), result);
            Assert.Contains(Path.Combine(BaseDirectory, "bin", "result.dat"), result);
            Assert.Contains(Path.Combine(BaseDirectory, "bin", "data", "temp.dat"), result);
            Assert.Contains(Path.Combine(BaseDirectory, "obj", "intermediate.dat"), result);
            Assert.Contains(Path.Combine(BaseDirectory, "some.info", "info.dat"), result);
        }
    }
}