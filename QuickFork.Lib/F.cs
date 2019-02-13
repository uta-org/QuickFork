using System;
using System.IO;
using System.Drawing;

using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;

using uzLib.Lite.Extensions;

using Newtonsoft.Json;

using Console = Colorful.Console;

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
        /// Retrieves the dependencies.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <returns></returns>
        public static CsProjLinking RetrieveDependencies(this ProjectItem pItem)
        {
            string packageFile = pItem.GetPackageFile();

            if (File.Exists(packageFile))
            {
                string contents = File.ReadAllText(packageFile);

                if (!contents.IsValidJSON())
                {
                    Console.WriteLine($"The {pItem.Name} project doesn't have a valid dependencies.json file.", Color.Yellow);
                    return new CsProjLinking();
                }

                return JsonConvert.DeserializeObject<CsProjLinking>(contents);
            }

            return new CsProjLinking();
        }

        /// <summary>
        /// Creates the dependencies.json file
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <returns></returns>
        public static void CreateDependencies(this ProjectItem pItem, bool forceNew = false)
        {
            string solutionPath = GetSolutionPath(pItem),
                   packageFile = pItem.GetPackageFile();

            if (File.Exists(packageFile))
            {
                Console.WriteLine("The 'dependencies.json' file already exists for this solution.", Color.Red);
                return;
            }

            var solution = SolutionParser.Parse(solutionPath) as Solution;
            CsProjLinking map = forceNew ? new CsProjLinking() : pItem.RetrieveDependencies();

            foreach (Project project in solution.Projects)
            {
                if (project == null)
                    continue;

                string repoPath;
                if (FindGitFolder(project.Path, out repoPath, solutionPath))
                {
                    string remoteUrl = GitHelper.GetRemoteUrl(Path.Combine(repoPath, ".git"));
                    map.AddLink(remoteUrl, Path.GetFileName(project.Path));
                }
            }

            map.SaveDependencies(packageFile);
        }

        public static bool FindGitFolder(string startingFolder, out string folderPath, string rootFolder = "")
        {
            if (!rootFolder.IsDirectory())
                rootFolder = Path.GetDirectoryName(rootFolder);

            string projectName = Path.GetFileName((string)startingFolder.Clone());

            if (string.IsNullOrEmpty(rootFolder) && !Path.IsPathRooted(startingFolder))
                throw new ArgumentNullException("rootFolder", "The startingFolder provided isn't rooted. This is obligatory due to the method logic.");

            string projPath;
            if (!Path.IsPathRooted(startingFolder))
            {
                // Search for the solution folder of the folder instead of using RootFolder
                projPath = Path.GetFullPath(Path.Combine(rootFolder, startingFolder));

                if (projPath.Contains(rootFolder))
                { // We are not interested on iterating self-folders so exit when needed
                    folderPath = "";
                    return false;
                }

                if (!projPath.IsDirectory())
                    projPath = Path.GetDirectoryName(projPath);

                if (!Directory.Exists(projPath))
                { // If we couldn't get the project path, then we would exit
                    Console.WriteLine($"The '{projectName}' project from this solution couldn't be found, check that you have then and where are you executing this!", Color.Yellow);

                    folderPath = "";
                    return false;
                }
            }
            else
            { // Un-tested
                projPath = startingFolder;

                if (!projPath.IsDirectory())
                    projPath = Path.GetDirectoryName(projPath);
            }

            do
            {
                if (Directory.Exists(Path.Combine(projPath, ".git")))
                {
                    folderPath = projPath;
                    return true;
                }

                projPath = Path.GetDirectoryName(projPath);
            }
            while (!string.IsNullOrEmpty(startingFolder));

            Console.WriteLine($"The indexed '{projectName}' project on solution doesn't have a repository on its folders. QuickFork can't create dependencies of non-repository projects yet!", Color.Yellow);

            folderPath = "";
            return false;
        }
    }
}