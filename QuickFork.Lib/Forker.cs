using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using uzLib.Lite.Extensions;

using System.Drawing;
using Console = Colorful.Console;

namespace QuickFork.Lib
{
    using Model;

    /// <summary>
    /// Forker class
    /// </summary>
    public static class Forker
    {
        /// <summary>
        /// Gets my settings.
        /// </summary>
        /// <value>
        /// My settings.
        /// </value>
        private static Settings MySettings => Settings.Default;

        /// <summary>
        /// Gets or sets the synchronize folder.
        /// </summary>
        /// <value>
        /// The synchronize folder.
        /// </value>
        public static string SyncFolder { get; set; }

        /// <summary>
        /// Gets or sets the repo map.
        /// </summary>
        /// <value>
        /// The repo map.
        /// </value>
        [JsonProperty]
        private static Dictionary<string, List<int>> RepoMap { get; set; }

        /// <summary>
        /// Gets the repos.
        /// </summary>
        /// <value>
        /// The repos.
        /// </value>
        [JsonIgnore]
        public static Dictionary<string, List<RepoItem>> Repos { get; private set; }

        /// <summary>
        /// Gets the stored projects.
        /// </summary>
        /// <value>
        /// The stored projects.
        /// </value>
        public static HashSet<ProjectItem> StoredProjects { get; private set; }

        /// <summary>
        /// Gets the stored repos.
        /// </summary>
        /// <value>
        /// The stored repos.
        /// </value>
        public static HashSet<RepoItem> StoredRepos { get; private set; }

        /// <summary>
        /// Gets or sets the repo proj linking.
        /// </summary>
        /// <value>
        /// The repo proj linking.
        /// </value>
        public static Dictionary<int, string[]> RepoProjLinking { get; private set; }

        public static void SerializeProject(ProjectItem pItem, RepoItem rItem, params string[] selectedProjects)
        {
            string packageFile = pItem.GetPackageFile();

            CsProjLinking map = pItem.RetrieveDependencies();

            if (!SerializationHelper.TryDeserialize(packageFile, out map))
            {
                Console.WriteLine($"Can't deserialize content from '{packageFile}'", Color.Red);
                return;
            }

            map.AddLink(rItem.GitUrl, selectedProjects);
            map.SaveDependencies(packageFile);
        }

        /// <summary>
        /// Determines whether [is already on file] [the specified file path].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="gitUrl">The git URL.</param>
        /// <returns>
        ///   <c>true</c> if [is already on file] [the specified file path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlreadyOnFile(string filePath, string gitUrl)
        {
            if (!File.Exists(filePath))
                return false;

            string contents = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<CsProjLinking>(contents);

            return obj.HasKey(gitUrl);
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public static void LoadSettings()
        {
            DoMapping(true);

            if (StoredProjects == null)
                StoredProjects = new HashSet<ProjectItem>();

            if (RepoProjLinking == null)
                RepoProjLinking = new Dictionary<int, string[]>();

            if (!string.IsNullOrEmpty(MySettings.StoredProjects) && MySettings.StoredProjects != "null")
                StoredProjects = JsonConvert.DeserializeObject<HashSet<ProjectItem>>(MySettings.StoredProjects);

            if (!string.IsNullOrEmpty(MySettings.RepoProjLinking) && MySettings.RepoProjLinking != "null")
                RepoProjLinking = JsonConvert.DeserializeObject<Dictionary<int, string[]>>(MySettings.RepoProjLinking);

            SyncFolder = MySettings.SyncFolder;
        }

        /// <summary>
        /// Does the mapping.
        /// </summary>
        /// <param name="isLoading">if set to <c>true</c> [is loading].</param>
        private static void DoMapping(bool isLoading)
        {
            if (isLoading)
            {
                string loadMapNeedle = MySettings.RepoMap,
                       loadRepoNeedle = MySettings.StoredRepos;

                if (!string.IsNullOrEmpty(loadMapNeedle) && loadMapNeedle != "null")
                    RepoMap = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(loadMapNeedle);

                if (!string.IsNullOrEmpty(loadRepoNeedle) && loadRepoNeedle != "null")
                {
                    StoredRepos = JsonConvert.DeserializeObject<HashSet<RepoItem>>(loadRepoNeedle);

                    //int i = 0;
                    //bool needsToSave = false;
                    //foreach (var repo in StoredRepos)
                    //{
                    //    if (repo.Index == -1)
                    //    {
                    //        repo.Index = i;
                    //        if (!needsToSave) needsToSave = true;
                    //    }

                    //    ++i;
                    //}

                    //if (needsToSave)
                    //    SaveStoredRepos();
                }
                else
                    StoredRepos = new HashSet<RepoItem>();

                if (Repos == null)
                {
                    if (RepoMap != null)
                        DoRemapping(false);
                    else
                        Repos = new Dictionary<string, List<RepoItem>>();
                }
            }
            else
            {
                // Mapping is no longer needed it's done in Add method
                SaveRepoMap(); // Not sure if there I need a true flag
            }
        }

        /// <summary>
        /// Does the remapping.
        /// </summary>
        /// <param name="fromValuesToMap">if set to <c>true</c> [from values to map].</param>
        public static void DoRemapping(bool fromValuesToMap = true)
        {
            if (fromValuesToMap)
                RepoMap = Repos.ToDictionary(t => t.Key, t => t.Value.Select(x => StoredRepos.IndexOf(x)).ToList());
            else
                Repos = RepoMap.ToDictionary(t => t.Key, t => t.Value.Select(x => StoredRepos.ElementAt(x)).ToList());
        }

        /// <summary>
        /// Adds the linking.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="projects">The projects.</param>
        public static void AddLinking(string projectPath, params string[] projects)
        {
            int? index = RepoMap?.Keys?.Select((k, i) => new { Index = i, ProjectPath = k })?.FirstOrDefault(a => a.ProjectPath == projectPath)?.Index;

            if (!index.HasValue)
                return;

            AddLinking(index.Value, projects);
        }

        /// <summary>
        /// Adds the linking.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="projects">The projects.</param>
        public static void AddLinking(int index, IEnumerable<string> projects)
        {
            AddLinking(index, projects.ToArray());
        }

        /// <summary>
        /// Adds the linking.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="projects">The projects.</param>
        public static void AddLinking(int index, params string[] projects)
        {
            if (RepoProjLinking.ContainsKey(index) && RepoProjLinking[index] == projects)
                return;

            RepoProjLinking.AddOrSet(index, projects);

            SaveRepoProjLinking();
        }

        /// <summary>
        /// Removes all linkings.
        /// </summary>
        /// <param name="repoIndex">Index of the repo.</param>
        public static void RemoveAllLinkings(ProjectItem pItem, int repoIndex)
        {
            if (!RepoProjLinking.ContainsKey(repoIndex))
                return;

            RepoProjLinking[repoIndex] = null;

            CheckIfEmptyLinking(pItem, repoIndex);

            SaveRepoProjLinking();
        }

        /// <summary>
        /// Removes the linking.
        /// </summary>
        /// <param name="repoIndex">Index of the repo.</param>
        /// <param name="projectIndex">Index of the project.</param>
        public static void RemoveLinking(ProjectItem pItem, int repoIndex, int projectIndex)
        {
            if (!RepoProjLinking.ContainsKey(repoIndex))
                return;

            RepoProjLinking[repoIndex] = RepoProjLinking[repoIndex].Where((val, index) => index != projectIndex).ToArray();

            CheckIfEmptyLinking(pItem, repoIndex);

            SaveRepoProjLinking();
        }

        /// <summary>
        /// Checks if empty linking.
        /// </summary>
        /// <param name="repoIndex">Index of the repo.</param>
        private static void CheckIfEmptyLinking(ProjectItem pItem, int repoIndex)
        {
            if (RepoProjLinking[repoIndex].IsNullOrEmpty())
            {
                RepoProjLinking.Remove(repoIndex);

                if (RepoMap[pItem.SelectedPath].Remove(repoIndex))
                    SaveRepoMap(true);
            }
        }

        /// <summary>
        /// Removes the linking.
        /// </summary>
        /// <param name="project">The project.</param>
        [Obsolete]
        public static void RemoveLinking(string project)
        {
            if (!RepoProjLinking.Any(kv => kv.Value.Contains(project)))
                return;

            int key = RepoProjLinking.FirstOrDefault(kv => kv.Value.Contains(project)).Key;

            RepoProjLinking[key] = RepoProjLinking[key].Where(arr => !arr.Contains(project)).ToArray();
        }

        /// <summary>
        /// Adds the specified project path.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        public static void Add(ProjectItem pItem)
        {
            if (!StoredProjects.Contains(pItem))
            {
                StoredProjects.Add(pItem);
                SaveStoredProjects();
            }
        }

        /// <summary>
        /// Adds the project path.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <param name="rItem">The repository item.</param>
        /// <returns>The index of the last added repository.</returns>
        public static void Add(ProjectItem pItem, RepoItem rItem)
        {
            // projectPath == string.Empty, this means that the project will not be linked
            string projectPath = pItem == null ? string.Empty : pItem.SelectedPath;

            if (!string.IsNullOrEmpty(projectPath))
                Add(pItem);

            if (!StoredRepos.Contains(rItem))
            {
                StoredRepos.Add(rItem);

                SaveStoredRepos();
            }

            if (RepoMap == null)
                RepoMap = new Dictionary<string, List<int>>();

            UpdateMap(projectPath, rItem);
        }

        public static void UpdateMap(string projectPath, RepoItem rItem, bool force = true)
        {
            if (!string.IsNullOrEmpty(projectPath))
            {
                if (!RepoMap.ContainsKey(projectPath))
                    RepoMap.Add(projectPath, new List<int>());

                int index = rItem.GetIndex();
                if (!RepoMap[projectPath].Contains(index) || force)
                {
                    // index.HasValue ? index.Value : (StoredRepos.Count > 0 ? StoredRepos.Select(r => r.Index).Last() + 1 : 0)
                    RepoMap[projectPath].Add(index);
                    SaveRepoMap();
                }
            }
        }

        /// <summary>
        /// Saves the instance.
        /// </summary>
        /// <exception cref="Exception">You deleted exe config file!</exception>
        public static void SaveInstance()
        {
            if (MySettings == null)
                throw new Exception("You deleted exe config file!");

            SaveSyncFolder(false);
            SaveStoredProjects(false);
            SaveStoredRepos(false);
            SaveRepoProjLinking(false);
            DoMapping(false);
        }

        /// <summary>
        /// Saves the synchronize folder.
        /// </summary>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        public static void SaveSyncFolder(bool fSave = true)
        {
            MySettings.SyncFolder = SyncFolder;

            if (fSave)
                MySettings.Save();
        }

        /// <summary>
        /// Saves the stored folders.
        /// </summary>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        public static void SaveStoredProjects(bool fSave = true)
        {
            MySettings.StoredProjects = JsonConvert.SerializeObject(StoredProjects);

            if (fSave)
                MySettings.Save();
        }

        /// <summary>
        /// Saves the repo map.
        /// </summary>
        public static void SaveRepoMap(bool doRemapping = false)
        {
            if (RepoMap == null)
                return;

            if (doRemapping)
                DoRemapping(false);

            MySettings.RepoMap = JsonConvert.SerializeObject(RepoMap);
            MySettings.Save();
        }

        /// <summary>
        /// Saves the stored repos.
        /// </summary>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        public static void SaveStoredRepos(bool fSave = true)
        {
            MySettings.StoredRepos = JsonConvert.SerializeObject(StoredRepos);

            if (fSave)
                MySettings.Save();
        }

        /// <summary>
        /// Saves the repo proj linking.
        /// </summary>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        public static void SaveRepoProjLinking(bool fSave = true)
        {
            MySettings.RepoProjLinking = JsonConvert.SerializeObject(RepoProjLinking);

            if (fSave)
                MySettings.Save();
        }
    }
}