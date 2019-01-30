using EasyConsole;
using Newtonsoft.Json;
using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uzLib.Lite.Interoperability;
using uzLib.Lite.Extensions;

namespace QuickFork.Lib.Model
{
    [Serializable]
    public class RepoItem
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
                    Console.WriteLine("Folder already exists, skipping...");
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
                        // .Where(p => !p.Contains("Demo") && !p.Contains("Test")); // <== No longer needed
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

                            int selectedProj = -1;
                            var csprojMenu = new Menu();

                            projs.ForEach((proj, i) => csprojMenu.Add(Path.GetFileNameWithoutExtension(proj), () => selectedProj = i));
                            csprojMenu.Add("Add all projects", () => selectedProj = -1);

                            csprojMenu.Display();

                            GetProjects(solution, out typeGuid, out projects);

                            if (selectedProj == -1)
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
                                solution.Projects = projects.ToList().AddAndGet(GetProject(projects, projectPath, projs[selectedProj], typeGuid, out alreadyExists));
                        }

                        File.WriteAllText(solutions[0], SolutionRenderer.Render(solution));
                        break;

                    case OperationType.CreateSymlink:
                        NativeMethods.CreateSymbolicLink(FolderPath, folderName, NativeEnums.SymbolicLinkFlags.Directory);
                        break;
                }

                if (!alreadyExists)
                {
                    Console.WriteLine($"Execution of '{operationType}' has been done succesfully!");
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

        private static Project GetProject(IEnumerable<Project> projects, string projectPath, string proj, Guid typeGuid, out bool alreadyExists)
        {
            string projectName;
            return GetProject(projects, projectPath, proj, typeGuid, out alreadyExists, out projectName);
        }

        private static Project GetProject(IEnumerable<Project> projects, string projectPath, string proj, Guid typeGuid, out bool alreadyExists, out string projectName)
        {
            string _projectName = Path.GetFileName(projectPath).Replace("\\", "").Replace("/", "");
            projectName = _projectName;

            if (projects.Any(p => p.Name == _projectName))
            {
                Console.WriteLine();
                Console.WriteLine("The project you are trying to add to solution already exists on solution.");
                Console.WriteLine();

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

        public void Update(string projectPath, bool fSave = true)
        {
            bool firstTime;
            Forker.Repos.Get(projectPath).InsertOrGet(new RepoItem(GitUrl), r => r.GitUrl == GitUrl, out firstTime);

            if (fSave && firstTime)
                Forker.SaveInstance();
        }

        public override string ToString()
        {
            return $"{Name} ({GitUrl})";
        }
    }
}