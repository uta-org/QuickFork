using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace QuickFork.Lib
{
    using Model;
    using System.Linq;

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
        public static StringCollection StoredProjects { get; private set; }

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

        public static bool IsAlreadyOnFile(string filePath, string projectPath)
        {
            if (!File.Exists(filePath))
                return false;

            var obj = JsonConvert.DeserializeObject<Dictionary<string, List<RepoItem>>>(File.ReadAllText(filePath));
            return obj.ContainsKey(projectPath);
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public static void LoadSettings()
        {
            DoMapping(true);

            if (StoredProjects == null)
                StoredProjects = new StringCollection();

            if (MySettings.StoredProjects == null)
            {
                MySettings.StoredProjects = new StringCollection();
                MySettings.Save();
            }
            else
                StoredProjects = MySettings.StoredProjects;

            SyncFolder = MySettings.SyncFolder;
        }

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
                        Repos = RepoMap.ToDictionary(t => t.Key, t => t.Value.Select(x => StoredRepos.ElementAt(x)).ToList());
                    else
                        Repos = new Dictionary<string, List<RepoItem>>();
                }
            }
            else
            {
                // Mapping is no longer needed it's done in Add method
                MySettings.Save();
            }
        }

        /// <summary>
        /// Adds the specified project path.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        public static void Add(string projectPath)
        {
            if (!StoredProjects.Contains(projectPath))
            {
                StoredProjects.Add(projectPath);
                SaveStoredProjects();
            }
        }

        /// <summary>
        /// Adds the project path.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="rItem">The repository item.</param>
        public static void Add(string projectPath, RepoItem rItem)
        {
            Add(projectPath);

            if (!StoredRepos.Contains(rItem))
            {
                StoredRepos.Add(rItem);
                SaveStoredRepos();

                if (RepoMap == null)
                    RepoMap = new Dictionary<string, List<int>>();

                if (!RepoMap.ContainsKey(projectPath))
                    RepoMap.Add(projectPath, new List<int>());

                RepoMap[projectPath].Add(StoredRepos.Count - 1);
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
            MySettings.StoredProjects = StoredProjects;

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