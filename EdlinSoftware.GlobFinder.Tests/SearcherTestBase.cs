using System.Collections.Generic;

namespace EdlinSoftware.FileSystemSearcher.Tests
{
    public abstract class SearcherTestBase
    {
        protected FakeFileSystem FileSystem;

        protected SearcherTestBase()
        {
            FileSystem = new FakeFileSystem
            {
                Directories =
                {
                    { @"", new List<string> { "bin", "obj", "some.info" } },
                    { @"bin", new List<string> { "data" } }
                },
                Files =
                {
                    { @"", new List<string> { "test.dat" } },
                    { @"bin", new List<string> { "result.dat" } },
                    { @"bin\data", new List<string> { "temp.dat" } },
                    { @"obj", new List<string> { "intermediate.dat" } },
                    { @"some.info", new List<string> { "info.dat" } }
                }
            };
        }
    }
}