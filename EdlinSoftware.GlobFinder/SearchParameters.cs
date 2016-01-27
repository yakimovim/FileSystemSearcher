using System.IO;
using System.Linq;

namespace EdlinSoftware.FileSystemSearcher
{
    internal class SearchParameters
    {
        public SearchParameters(string baseDirectory, string template)
        {
            if (Path.IsPathRooted(template))
            {
                var pathRoot = Path.GetPathRoot(template);

                var searchTemplateParts = template.Substring((pathRoot ?? string.Empty).Length)
                    .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                BaseDirectory = pathRoot;
                SearchTemplateParts = searchTemplateParts;
            }
            else
            {
                var searchTemplateParts = template
                    .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (searchTemplateParts[0] == ".")
                    searchTemplateParts = searchTemplateParts.Skip(1).ToArray();

                BaseDirectory = baseDirectory;
                SearchTemplateParts = searchTemplateParts;
            }

            MoveTemplatePartsWithoutWildCards();
        }

        public string BaseDirectory { get; private set; }
        public string[] SearchTemplateParts { get; private set; }

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