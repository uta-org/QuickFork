using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace QuickFork.Lib
{
    using Model;

    //[Serializable]
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
        /// Gets the repos.
        /// </summary>
        /// <value>
        /// The repos.
        /// </value>
        public static Dictionary<string, List<RepoItem>> Repos { get; private set; }

        /// <summary>
        /// Gets the stored folders.
        /// </summary>
        /// <value>
        /// The stored folders.
        /// </value>
        public static StringCollection StoredFolders { get; private set; }

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
            string loadString = MySettings.Repos;

            if (!string.IsNullOrEmpty(loadString))
                Repos = JsonConvert.DeserializeObject<Dictionary<string, List<RepoItem>>>(loadString);

            if (Repos == null)
                Repos = new Dictionary<string, List<RepoItem>>();

            if (StoredFolders == null)
                StoredFolders = new StringCollection();

            if (MySettings.StoredFolders == null)
            {
                MySettings.StoredFolders = new StringCollection();
                MySettings.Save();
            }
            else
                StoredFolders = MySettings.StoredFolders;

            SyncFolder = MySettings.SyncFolder;
        }

        /// <summary>
        /// Adds the specified project path.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        public static void Add(string projectPath)
        {
            if (!StoredFolders.Contains(projectPath))
            {
                StoredFolders.Add(projectPath);
                SaveStoredFolders();
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

            if (!Repos.ContainsKey(projectPath))
                Repos.Add(projectPath, new List<RepoItem>());

            Repos[projectPath].Add(rItem);
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
            SaveStoredFolders(false);
            SaveRepos();
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
        public static void SaveStoredFolders(bool fSave = true)
        {
            MySettings.StoredFolders = StoredFolders;

            if (fSave)
                MySettings.Save();
        }

        /// <summary>
        /// Saves the repo collection.
        /// </summary>
        /// <param name="fSave">if set to <c>true</c> [f save].</param>
        public static void SaveRepos(bool fSave = true)
        {
            MySettings.Repos = JsonConvert.SerializeObject(Repos);

            if (fSave)
                MySettings.Save();
        }
    }
}