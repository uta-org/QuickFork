using EasyConsole;
using Newtonsoft.Json;
using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using uzLib.Lite.Interoperability;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Lib.Model
{
    using Interfaces;

    [Serializable]
    public class RepoItem : IModel
    {
        public static GitShell MyShell { get; private set; }

        [JsonProperty]
        public string GitUrl { get; set; }

        [JsonIgnore]
        public string Name => GitUrl.GetFileNameFromUrlWithoutExtension();

        private RepoItem()
        {
            MyShell = new GitShell();
        }

        public RepoItem(string gitUrl, bool fSave = true)
            : this()
        {
            if (!Uri.IsWellFormedUriString(gitUrl, UriKind.RelativeOrAbsolute))
                throw new Exception("Invalid url given");

            GitUrl = gitUrl;
        }

        public async void Execute(string projectPath, OperationType operationType = OperationType.AddProjToSLN, bool? doLinking = null)
        {
            string folderName = Name,
                   FolderPath = Path.Combine(Settings.Default.SyncFolder, folderName),
                   workingPath = Settings.Default.SyncFolder;

            if (!doLinking.HasValue || doLinking.HasValue && !doLinking.Value)
            {
                if (!Directory.Exists(FolderPath))
                {
                    MyShell.CurrentInfo.WorkingDirectory = workingPath;
                    await MyShell.SendCommand($"clone {GitUrl} {folderName}");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Folder already exists, skipping...", Color.Yellow);
                    Console.WriteLine();
                }
            }

            if (!doLinking.HasValue || doLinking.HasValue && doLinking.Value)
            {
                bool alreadyExists = false;

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

                        int projCount = projs.Count();

                        Guid typeGuid;
                        IEnumerable<Project> projects;

                        if (projCount == 0)
                            throw new Exception($"The cloned repo '{Path.GetFileName(FolderPath)}' doesn't have any *.csproj files. If you need to support another kind of project, please, fork this project and implement it!");
                        else if (projCount == 1)
                        {
                            GetProjects(solution, out typeGuid, out projects);
                            solution.Projects = projects.ToList().AddAndGet(GetProject(projects, projectPath, projs.First(), typeGuid, out alreadyExists));
                        }
                        else
                        {
                            // Let user choose one, several or all csprojs.

                            List<int> selectedProjs = new List<int>();
                            var csprojMenu = new Menu();

                            csprojMenu.AddRange(projs.Select((proj, i) => new Option(Path.GetFileNameWithoutExtension(proj), () => selectedProjs.Add(i))));
                            csprojMenu.Add("Add all projects", () => selectedProj = null);

                            csprojMenu.Display(true);

                            if (selectedProjs.Contains(csprojMenu.Count - 1))
                                selectedProjs = null;

                            GetProjects(solution, out typeGuid, out projects);

                            if (selectedProjs == null)
                            {
                                List<string> projectNames = new List<string>();

                                solution.Projects = projects.ToList().AddRangeAndGet(projs.Select(proj =>
                                {
                                    string projectName;

                                    var _proj = GetProject(projects, projectPath, proj, typeGuid, out alreadyExists, out projectName);
                                    projectNames.Add(projectName);

                                    return _proj;
                                })
                                .Where(p => !projectNames.Contains(p.Name)));
                            }
                            else
                            {
                                solution.Projects = projects
                                    .ToList()
                                    .AddRangeAndGet(selectedProjs.Select(selectedProj => GetProject(projects, projectPath, projs[selectedProj], typeGuid, out alreadyExists)));
                            }
                        }

                        File.WriteAllText(solutions[0], SolutionRenderer.Render(solution));
                        break;

                    case OperationType.CreateSymlink:
                        NativeMethods.CreateSymbolicLink(FolderPath, folderName, NativeEnums.SymbolicLinkFlags.Directory);
                        break;
                }

                if (!alreadyExists)
                {
                    Console.WriteLine($"Execution of '{operationType}' has been done succesfully!", Color.Green);
                    Console.WriteLine();
                }
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

        private static Project GetProject(IEnumerable<Project> projects, string projectPath, string proj, Guid typeGuid, out bool alreadyExists, bool promptWarning = true)
        {
            string projectName;
            return GetProject(projects, projectPath, proj, typeGuid, out alreadyExists, out projectName, promptWarning);
        }

        private static Project GetProject(IEnumerable<Project> projects, string projectPath, string proj, Guid typeGuid, out bool alreadyExists, out string projectName, bool promptWarning = true)
        {
            string _projectName = Path.GetFileName(projectPath).Replace("\\", "").Replace("/", "");
            projectName = _projectName;

            if (projects.Any(p => p.Name == _projectName))
            {
                if (promptWarning)
                {
                    Console.WriteLine();
                    Console.WriteLine("The project you are trying to add to solution already exists on solution. Skipping...", Color.Yellow);
                    Console.WriteLine();
                }

                alreadyExists = true;
                return null;
            }

            alreadyExists = false;
            return new Project(
                                            typeGuid,
                                            projectName,
                                            !IOHelper.IsRelative(projectPath, proj) ? proj : IOHelper.MakeRelativePath(projectPath, proj),
                                            Guid.NewGuid());
        }

        public static RepoItem Update(string gitUrl, bool fSave = true)
        {
            return Update(null, gitUrl, fSave);
        }

        public static RepoItem Update(ProjectItem pItem, string gitUrl, bool fSave = true)
        {
            RepoItem rItem = null;
            bool firstTime = true;

            if (!Forker.StoredRepos.IsNullOrEmpty())
            {
                rItem = Forker.StoredRepos.FirstOrDefault(r => r.GitUrl == gitUrl);
                firstTime = rItem == null;
            }

            if (firstTime)
            {
                rItem = new RepoItem(gitUrl);
                Forker.Add(pItem, rItem);
            }
            else
            {
                Console.WriteLine("The repository you're trying to add is already present on collection. Skipping...", Color.Yellow);
                Console.WriteLine();

                return rItem;
            }

            if (fSave && firstTime)
                Forker.SaveInstance();

            return rItem;
        }

        public override string ToString()
        {
            return $"{Name} ({GitUrl})";
        }
    }
}