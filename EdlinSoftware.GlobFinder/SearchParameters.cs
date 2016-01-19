using System.IO;
using System.Linq;

namespace EdlinSoftware.FileSystemSearcher
{
    internal class SearchParameters
    {
        public SearchParameters(string baseDirectory, string template)
        {
            var searchTemplateParts = template.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (IsAbsolutePath(searchTemplateParts))
            {
                BaseDirectory = searchTemplateParts[0] + Path.DirectorySeparatorChar;
                SearchTemplateParts = searchTemplateParts.Skip(1).ToArray();
            }
            else if (IsNetworkPath(template))
            {
                BaseDirectory = @"\\" + searchTemplateParts[2] + @"\";
                SearchTemplateParts = searchTemplateParts.Skip(3).ToArray();
            }
            else
            {
                if (searchTemplateParts[0] == ".")
                    searchTemplateParts = searchTemplateParts.Skip(1).ToArray();

                BaseDirectory = baseDirectory;
                SearchTemplateParts = searchTemplateParts;
            }

            MoveTemplatePartsWithoutWildCards();
        }

        public string BaseDirectory { get; private set; }
        public string[] SearchTemplateParts { get; private set; }

        private bool IsAbsolutePath(string[] searchTemplateParts)
        {
            return searchTemplateParts[0].EndsWith(Path.VolumeSeparatorChar.ToString());
        }

        private bool IsNetworkPath(string template)
        {
            return template.StartsWith(@"\\");
        }

        private void MoveTemplatePartsWithoutWildCards()
        {
            int partsToSkip = 0;

            for (int i = 0; i < SearchTemplateParts.Length - 1; i++)
            {
                if (!HasWildCard(SearchTemplateParts[i]))
                {
                    BaseDirectory = Path.Combine(BaseDirectory, SearchTemplateParts[i]);
                    partsToSkip++;
                }
                else
                {
                    break;
                }
            }

            SearchTemplateParts = SearchTemplateParts.Skip(partsToSkip).ToArray();
        }

        private bool HasWildCard(string searchTemplatePart)
        {
            return searchTemplatePart.Contains("*")
                   || searchTemplatePart.Contains("?");
        }
    }
}