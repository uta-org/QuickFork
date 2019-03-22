using EasyConsole;
using Newtonsoft.Json;
using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using uzLib.Lite.Extensions;
using uzLib.Lite.Plugins.SymLinker;
using Console = Colorful.Console;

namespace QuickFork.Lib.Model
{
    using Interfaces;
    using Properties;

    /// <summary>
    /// The RepoItem class
    /// </summary>
    [Serializable]
    public class RepoItem : IModel
    {
        /// <summary>
        /// Gets or sets the git URL.
        /// </summary>
        /// <value>
        /// The git URL.
        /// </value>
        [JsonProperty]
        public string GitUrl { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonIgnore]
        public string Name => GitUrl.GetFileNameFromUrlWithoutExtension();

        /// <summary>
        /// A list of CSharp Projects
        /// </summary>
        private static List<string> CsProjs { get; set; }

        /// <summary>
        /// A static constructor for RepoItem
        /// </summary>
        static RepoItem()
        {
            CsProjs = new List<string>();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="RepoItem"/> class from being created.
        /// </summary>
        private RepoItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoItem"/> class.
        /// </summary>
        /// <param name="gitUrl">The git URL.</param>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        /// <exception cref="Exception">Invalid url given</exception>
        public RepoItem(string gitUrl, bool fSave = true)
            : this()
        {
            if (!Uri.IsWellFormedUriString(gitUrl, UriKind.RelativeOrAbsolute))
                throw new Exception("Invalid url given");

            GitUrl = gitUrl;
        }

        /// <summary>
        /// Executes the specified p item.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="doLinking">The do linking.</param>
        /// <returns></returns>
        /// <exception cref="Exception">The cloned repo '{Path.GetFileName(FolderPath)}</exception>
        public string[] Execute(ProjectItem pItem, OperationType operationType = OperationType.AddProjToSLN, bool? doLinking = null)
        {
            // TODO: Refactorize this

            // Clear the project list
            CsProjs.Clear();

            string folderName = Name,
                   FolderPath = Path.Combine(Settings.Default.SyncFolder, folderName),
                   workingPath = Settings.Default.SyncFolder;

            // Clone repo

            if (!doLinking.HasValue || doLinking.HasValue && !doLinking.Value)
            {
                if (!Directory.Exists(FolderPath))
                    GitHelper.CloneRepo(workingPath, GitUrl, folderName);
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Folder already exists, skipping...", Color.Yellow);
                    Console.WriteLine();
                }
            }

            // Link repository to solution

            if (!doLinking.HasValue || doLinking.HasValue && doLinking.Value)
            {
                bool alreadyExists = false;

                switch (operationType)
                {
                    case OperationType.AddProjToSLN:
                        // Patch unresolved projects
                        var tuple = PatchSolution(pItem);

                        string solutionPath = tuple.Item1;
                        Solution solution = tuple.Item2;

                        // Continue linking csprojs (from repo) to desired solution

                        var projs = Directory.GetFiles(FolderPath, "*.csproj", SearchOption.AllDirectories);

                        int projCount = projs.Count();

                        Guid typeGuid;
                        IEnumerable<Project> projects;

                        if (projCount == 0)
                            throw new Exception($"The cloned repo '{Path.GetFileName(FolderPath)}' doesn't have any *.csproj files. If you need to support another kind of project, please, fork this project and implement it!");
                        else if (projCount == 1)
                        {
                            GetProjects(solution, out typeGuid, out projects);
                            solution.Projects = projects.ToList().AddAndGet(GetProject(projects, pItem.SelectedPath, Path.GetFileName(projs.First()), projs.First(), typeGuid, out alreadyExists));

                            Forker.AddLinking(GetIndex(), Path.GetFileName(projs.First()));
                        }
                        else
                        {
                            // Let user choose one, several or all csprojs.

                            List<int> selectedProjs = new List<int>();
                            var csprojMenu = new Menu();

                            csprojMenu.AddRange(projs.Select((proj, i) => new Option(Path.GetFileNameWithoutExtension(proj), () => selectedProjs.Add(i))));
                            csprojMenu.Add("Add all projects", () => selectedProjs = null);

                            csprojMenu.Display(true);

                            if (selectedProjs.Contains(csprojMenu.Count - 1))
                                selectedProjs = null;

                            GetProjects(solution, out typeGuid, out projects);

                            if (selectedProjs == null)
                            {
                                List<string> projectNames = new List<string>();

                                solution.Projects = projects.ToList().AddRangeAndGet(projs.Select(projectPath =>
                                {
                                    string projectName = Path.GetFileName(projectPath);

                                    var _proj = GetProject(projects, pItem.SelectedPath, projectName, projectPath, typeGuid, out alreadyExists);
                                    projectNames.Add(projectName);

                                    return _proj;
                                })
                                .Where(p => !projectNames.Contains(p.Name)));

                                Forker.AddLinking(GetIndex(), projs.Select(proj => Path.GetFileName(proj)));
                            }
                            else
                            {
                                solution.Projects = projects
                                    .ToList()
                                    .AddRangeAndGet(selectedProjs.Select(selectedProj => GetProject(projects, pItem.SelectedPath, Path.GetFileName(projs[selectedProj]), projs[selectedProj], typeGuid, out alreadyExists)));

                                Forker.AddLinking(GetIndex(), selectedProjs.Select(sp => Path.GetFileName(projs[sp])));
                            }
                        }

                        File.WriteAllText(solutionPath, SolutionRenderer.Render(solution));
                        break;

                    case OperationType.CreateSymlink:
                        var linker = new Linker();
                        linker.CreateLink(FolderPath, folderName);
                        break;
                }

                if (!alreadyExists)
                {
                    Console.WriteLine($"Execution of '{operationType}' has been done succesfully!", Color.DarkGreen);
                    Console.WriteLine();
                }
            }

            return CsProjs.ToArray();
        }

        /// <summary>
        /// Patches the solution. (Detect if the solution has any unresolved nested project, by unresolved we mean projects that aren't available on the dependencies.json and has a git repo)
        /// </summary>
        /// <param name="solution">The solution.</param>
        private Tuple<string, Solution> PatchSolution(ProjectItem pItem)
        {
            string solutionPath = F.GetSolutionPath(pItem);
            Solution solution = SolutionParser.Parse(solutionPath) as Solution;

            // Forker.Repos.ContainsKey(pItem.SelectedPath) && !Forker.Repos[pItem.SelectedPath].Any(r => r.GitUrl == GitUrl) // Idk why I did this inside of the project loop
            // Basically, to check if the project was already on the collection to avoid dependencies creation (CreateDependencies) be balled multiple times.

            pItem.CreateDependencies();

            return new Tuple<string, Solution>(solutionPath, solution);
        }

        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="typeGuid">The type unique identifier.</param>
        /// <param name="projects">The projects.</param>
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

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <param name="projects">The projects.</param>
        /// <param name="workingPath">The working path.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="projectPath">The project path.</param>
        /// <param name="typeGuid">The type unique identifier.</param>
        /// <param name="alreadyExists">if set to <c>true</c> [already exists].</param>
        /// <param name="promptWarning">if set to <c>true</c> [prompt warning].</param>
        /// <returns></returns>
        private static Project GetProject(IEnumerable<Project> projects, string workingPath, string projectName, string projectPath, Guid typeGuid, out bool alreadyExists, bool promptWarning = true)
        {
            CsProjs.Add(projectPath);

            if (projects.Any(p => p.Name == projectName))
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
                                            !IOHelper.IsRelative(workingPath, projectPath) ? projectPath : IOHelper.MakeRelativePath(workingPath, projectPath),
                                            Guid.NewGuid());
        }

        /// <summary>
        /// Updates the specified git URL.
        /// </summary>
        /// <param name="gitUrl">The git URL.</param>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        /// <returns></returns>
        public static RepoItem Update(string gitUrl, bool fSave = true)
        {
            return Update(null, gitUrl, fSave);
        }

        /// <summary>
        /// Updates the specified p item.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <param name="gitUrl">The git URL.</param>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Name} ({GitUrl})";
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return Forker.StoredRepos.IndexOf(this);
        }
    }
}