using System.IO;

namespace QuickFork.Lib
{
    using Model;

    public static class F
    {
        public static string GetPackageFile(this ProjectItem pItem)
        {
            return Path.Combine(pItem.SelectedPath, "dependencies.json");
        }
    }
}