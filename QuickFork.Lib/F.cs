using Newtonsoft.Json;
using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using System;
using System.Drawing;
using System.IO;
using uzLib.Lite.Extensions;
using Console = Colorful.Console;

namespace QuickFork.Lib
{
    using Model;

    /// <summary>
    /// The main extensions
    /// </summary>
    public static class F
    {
        /// <summary>
        /// Gets the package file.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <returns></returns>
        public static string GetPackageFile(this ProjectItem pItem)
        {
            return GetPackageFile(pItem.SelectedPath);
        }

        /// <summary>
        /// Gets the package file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string GetPackageFile(string path)
        {
            return Path.Combine(path, "dependencies.json");
        }

        /// <summary>
        /// Determines whether the specified path has dependencies.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if the specified path has dependencies; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDependencies(string path)
        {
            return File.Exists(GetPackageFile(path));
        }

        /// <summary>
        /// Gets the solution path.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// There is any solution available yet!
        /// or
        /// Multiple solutions isn't supported yet!
        /// </exception>
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
        /// <param name="throwEx">if set to <c>true</c> [throw ex].</param>
        /// <returns></returns>
        public static CsProjLinking RetrieveDependencies(this ProjectItem pItem, bool throwEx = false)
        {
            return RetrieveDependencies(pItem.SelectedPath, pItem.Name, throwEx);
        }

        /// <summary>
        /// Retrieves the dependencies.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="throwEx">if set to <c>true</c> [throw ex].</param>
        /// <returns></returns>
        public static CsProjLinking RetrieveDependencies(string projectPath, bool throwEx = false)
        {
            return RetrieveDependencies(projectPath, Path.GetFileName(projectPath), throwEx);
        }

        /// <summary>
        /// Retrieves the dependencies.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="projName">Name of the proj.</param>
        /// <param name="throwEx">if set to <c>true</c> [throw ex].</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static CsProjLinking RetrieveDependencies(string projectPath, string projName, bool throwEx = false)
        {
            string packageFile = GetPackageFile(projectPath);

            if (File.Exists(packageFile))
            {
                string contents = File.ReadAllText(packageFile);

                if (!contents.IsValidJSON())
                {
                    string msg = $"The {projName} project doesn't have a valid dependencies.json file.";

                    if (throwEx)
                        throw new Exception(msg);
                    else
                        Console.WriteLine(msg, Color.Yellow);

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
                    map.AddLink(remoteUrl, project.Path);
                }
            }

            map.SaveDependencies(packageFile);
        }

        /// <summary>
        /// Finds the git folder.
        /// </summary>
        /// <param name="startingFolder">The starting folder.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="rootFolder">The root folder.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rootFolder - The startingFolder provided isn't rooted. This is obligatory due to the method logic.</exception>
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
                    Console.WriteLine($"The '{projectName}' project from this solution couldn't be found, please check that you have then and where are you executing this!", Color.Yellow);

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