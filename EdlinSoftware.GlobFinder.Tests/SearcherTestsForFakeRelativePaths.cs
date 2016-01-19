using System.IO;
using System.Linq;
using Xunit;

namespace EdlinSoftware.FileSystemSearcher.Tests
{
    public class SearcherTestsForFakeRelativePaths : SearcherTestBase
    {
        private readonly Searcher _searcher;

        public SearcherTestsForFakeRelativePaths()
        {
            FileSystem.BaseDirectory = @"c:\Some\Base\Directory\";
            _searcher = new Searcher(FileSystem) {BaseDirectory = FileSystem.BaseDirectory};
        }

        [Fact]
        public void Find_DirectFileName_AbsentFile()
        {
            var result = _searcher.Find("System.dll");

            Assert.False(result.Any(), "Absent file should not be found");
        }

        [Fact]
        public void Find_DirectFileName_ExistingFile()
        {
            var result = _searcher.Find("test.dat").ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(_searcher.BaseDirectory, "test.dat"), result[0]);
        }

        [Fact]
        public void Find_TemplateFileName_AbsentFile()
        {
            var result = _searcher.Find("a*t.dat").ToArray();

            Assert.False(result.Any(), "Absent files should not be found");
        }

        [Fact]
        public void Find_TemplateFileName_ExistingFile()
        {
            var result = _searcher.Find("t*t.dat").ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(_searcher.BaseDirectory, "test.dat"), result[0]);
        }

        [Fact]
        public void Find_DirectoryInTemplate_AbsentFile()
        {
            var result = _searcher.Find(@"bin\System.dll");

            Assert.False(result.Any(), "Absent file should not be found");
        }

        [Fact]
        public void Find_DirectoryInTemplate_ExistingFile()
        {
            var result = _searcher.Find(@"bin\result.dat").ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(_searcher.BaseDirectory, @"bin\result.dat"), result[0]);
        }

        [Fact]
        public void Find_UsingDot_ExistingFile()
        {
            var result = _searcher.Find(@".\bin\result.dat").ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(_searcher.BaseDirectory, @"bin\result.dat"), result[0]);
        }

        [Fact]
        public void Find_WildCardInDirectoryInTemplate_ExistingFile()
        {
            var result = _searcher.Find(@"b?n\result.dat").ToArray();

            Assert.Equal(1, result.Length);

            Assert.Equal(Path.Combine(_searcher.BaseDirectory, @"bin\result.dat"), result[0]);
        }

        [Fact]
        public void Find_AllDirectories_ExistingFiles()
        {
            var result = _searcher.Find(@"*/*.dat").ToArray();

            Assert.Equal(3, result.Length);

            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"bin\result.dat"), result);
            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"obj\intermediate.dat"), result);
            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"some.info\info.dat"), result);
        }

        [Fact]
        public void Find_AllSubDirectories_ExistingFiles()
        {
            var result = _searcher.Find(@"**/*.dat").ToArray();

            Assert.Equal(5, result.Length);

            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"test.dat"), result);
            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"bin\result.dat"), result);
            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"bin\data\temp.dat"), result);
            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"obj\intermediate.dat"), result);
            Assert.Contains(Path.Combine(_searcher.BaseDirectory, @"some.info\info.dat"), result);
        }
    }
}