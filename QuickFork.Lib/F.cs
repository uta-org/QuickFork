using System;
using System.IO;
using System.Threading.Tasks;

using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;

using uzLib.Lite.Interoperability;

namespace QuickFork.Lib
{
    using Model;

    public static class F
    {
        public static string GetPackageFile(this ProjectItem pItem)
        {
            return Path.Combine(pItem.SelectedPath, "dependencies.json");
        }

        public static string GetSolutionPath(this ProjectItem pItem)
        {
            string[] solutions = Directory.GetFiles(pItem.SelectedPath, "*.sln", SearchOption.AllDirectories);

            if (solutions.Length == 0)
                throw new Exception("There is any solution available yet!");
            else if (solutions.Length > 1)
                throw new Exception("Multiple solutions isn't supported yet!");

            // This project will only read the first solution that is found.
            return solutions[0];
        }

        /// <summary>
        /// Creates the dependencies.json file
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <returns></returns>
        public static async Task CreateDependencies(this GitShell MyShell, ProjectItem pItem)
        {
            string solutionPath = GetSolutionPath(pItem);

            var solution = SolutionParser.Parse(solutionPath) as Solution;
            CsProjLinking map = new CsProjLinking();

            foreach (Project project in solution.Projects)
            {
                string gitPath;
                if (FindGitFolder(project.Path, out gitPath))
                {
                    Task<string> commandTask = MyShell.ReadCommand($@"--git-dir=""{Path.Combine(gitPath, ".git")}"" --work-tree=""{gitPath}"" config --get remote.origin.url");
                    await commandTask;

                    string remoteUrl = commandTask.Result;
                    map.AddLink(remoteUrl, Path.GetFileName(project.Path));
                }
            }

            Forker.SerializeProject(pItem.GetPackageFile(), map);
        }

        public static bool FindGitFolder(string startingFolder, out string folderPath)
        {
            do
            {
                if (Directory.Exists(Path.Combine(startingFolder, ".git")))
                {
                    folderPath = startingFolder;
                    return true;
                }

                startingFolder = Path.GetDirectoryName(startingFolder);
            }
            while (!string.IsNullOrEmpty(startingFolder));

            folderPath = "";
            return false;
        }
    }
}