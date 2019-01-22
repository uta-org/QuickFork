using EasyConsole;
using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uzLib.Lite;
using uzLib.Lite.Extensions;

namespace QuickFork.Lib
{
    public class RepoItem
    {
        public static GitShell MyShell { get; private set; }

        public string GitUrl { get; private set; }

        private RepoItem()
        {
            MyShell = new GitShell();
        }

        public RepoItem(string gitUrl)
            : this()
        {
            if (!Uri.IsWellFormedUriString(gitUrl, UriKind.RelativeOrAbsolute))
                throw new Exception("Invalid url given");

            GitUrl = gitUrl;
        }

        public async void Execute(string projectPath, OperationType operationType = OperationType.AddProjToSLN, bool? doLinking = null)
        {
            string folderName = GitUrl.GetFileNameFromUrlWithoutExtension(),
                   FolderPath = Path.Combine(Settings.Default.SyncFolder, folderName),
                   workingPath = Settings.Default.SyncFolder;

            if (!Directory.Exists(FolderPath))
            {
                if (!doLinking.HasValue || doLinking.HasValue && !doLinking.Value)
                {
                    MyShell.CurrentInfo.WorkingDirectory = workingPath;
                    await MyShell.SendCommand($"clone {GitUrl} {folderName}");
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Folder already exists, skipping...");
                Console.WriteLine();
            }

            if (!doLinking.HasValue || doLinking.HasValue && doLinking.Value)
            {
                switch (operationType)
                {
                    case OperationType.AddProjToSLN:
                        string[] solutions = Directory.GetFiles(projectPath, "*.sln", SearchOption.AllDirectories);

                        if (solutions.Length == 0)
                            throw new Exception("There is any solution available yet!");
                        else if (solutions.Length > 1)
                            throw new Exception("Multiple solutions isn't supported yet!");

                        var solution = SolutionParser.Parse(solutions[0]) as Solution;
                        var projs = Directory.GetFiles(FolderPath, "*.csproj", SearchOption.AllDirectories);
                        // .Where(p => !p.Contains("Demo") && !p.Contains("Test")); // <== No longer needed
                        int projCount = projs.Count();

                        Guid typeGuid;
                        IEnumerable<Project> projects;

                        if (projCount == 0)
                            throw new Exception($"The cloned repo '{Path.GetFileName(FolderPath)}' doesn't have any *.csproj files. If you need to support another kind of project, please, fork this project and implement it!");
                        else if (projCount == 1)
                        {
                            GetProjects(solution, out typeGuid, out projects);

                            solution.Projects = projects.ToList().AddAndGet(GetProject(projectPath, workingPath, projs.First(), typeGuid));

                            File.WriteAllText(solutions[0], SolutionRenderer.Render(solution));
                        }
                        else
                        {
                            // Let user choose one, several or all csprojs.

                            int selectedProj = -1;
                            var csprojMenu = new Menu();

                            projs.ForEach((proj, i) => csprojMenu.Add(Path.GetFileNameWithoutExtension(proj), () => selectedProj = i));
                            csprojMenu.Add("Add all projects", () => selectedProj = -1);

                            csprojMenu.Display();

                            GetProjects(solution, out typeGuid, out projects);

                            if (selectedProj == -1)
                                solution.Projects = projects.ToList().AddRangeAndGet(projs.Select(proj => GetProject(projectPath, workingPath, proj, typeGuid)));
                            else
                                solution.Projects = projects.ToList().AddAndGet(GetProject(projectPath, workingPath, projs[selectedProj], typeGuid));
                        }

                        break;

                    case OperationType.CreateSymlink:
                        NativeMethods.CreateSymbolicLink(FolderPath, folderName, NativeEnums.SymbolicLinkFlags.Directory);
                        break;
                }

                Console.WriteLine($"Execution of '{operationType}' has been done succesfully!");
                Console.WriteLine();
            }
        }

        private static void GetProjects(Solution solution, out Guid typeGuid, out IEnumerable<Project> projects)
        {
            if (solution.Projects.IsNullOrEmpty())
            {
                projects = new List<Project>();
                typeGuid = Guid.NewGuid();
            }
            else
            {
                projects = solution.Projects;
                typeGuid = projects.First().TypeGuid;
            }
        }

        private static Project GetProject(string projectPath, string workingPath, string proj, Guid typeGuid)
        {
            return new Project(
                                            typeGuid,
                                            Path.GetFileNameWithoutExtension(projectPath),
                                            !IOHelper.IsRelative(workingPath, proj) ? proj : IOHelper.MakeRelativePath(workingPath, proj),
                                            Guid.NewGuid());
        }

        public override string ToString()
        {
            return $"{GitUrl.GetFileNameFromUrlWithoutExtension()} ({GitUrl})";
        }
    }
}