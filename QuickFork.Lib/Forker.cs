using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using uzLib.Lite.Extensions;

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

        public static string SerializeProject(string projectPath)
        {
            if (!Repos.ContainsKey(projectPath))
                throw new Exception("Can't serialize provided path (it's not present on Dictionary)!");

            return JsonConvert.SerializeObject(Repos[projectPath], Formatting.Indented);
        }

        public static bool IsAlreadyOnFile(string filePath, string gitUrl)
        {
            if (!File.Exists(filePath))
                return false;

            string contents = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<List<RepoItem>>(contents);

            return obj.Any(r => r.GitUrl == gitUrl);
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public static void LoadSettings()
        {
            DoMapping(true);

            if (StoredProjects == null)
                StoredProjects = new HashSet<ProjectItem>();

            if (!string.IsNullOrEmpty(MySettings.StoredProjects) && MySettings.StoredProjects != "null")
                StoredProjects = JsonConvert.DeserializeObject<HashSet<ProjectItem>>(MySettings.StoredProjects);

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
                    StoredRepos = JsonConvert.DeserializeObject<HashSet<RepoItem>>(loadRepoNeedle);
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

                SaveRepoMap();
                MySettings.Save();
            }
        }

        public static void DoRemapping(bool fromValuesToMap = true)
        {
            if (fromValuesToMap)
                RepoMap = Repos.ToDictionary(t => t.Key, t => t.Value.Select(x => StoredRepos.IndexOf(x)).ToList());
            else
                Repos = RepoMap.ToDictionary(t => t.Key, t => t.Value.Select(x => StoredRepos.ElementAt(x)).ToList());
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
        /// <param name="projectPath">The project path.</param>
        /// <param name="rItem">The repository item.</param>
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

                if (RepoMap == null)
                    RepoMap = new Dictionary<string, List<int>>();

                if (!string.IsNullOrEmpty(projectPath))
                {
                    if (!RepoMap.ContainsKey(projectPath))
                        RepoMap.Add(projectPath, new List<int>());

                    RepoMap[projectPath].Add(StoredRepos.Count - 1);
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
        public static void SaveRepoMap()
        {
            if (RepoMap == null)
                return;

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
    }
}