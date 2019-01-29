using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace QuickFork.Lib
{
    using Model;

    [Serializable]
    public class Forker
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

        /// <summary>
        /// Gets the repo collection.
        /// </summary>
        /// <value>
        /// The repo collection.
        /// </value>
        [JsonProperty]
        public List<RepoItem> RepoCollection { get; private set; }

        /// <summary>
        /// Gets the project path.
        /// </summary>
        /// <value>
        /// The project path.
        /// </value>
        [JsonProperty]
        public string ProjectPath { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="Forker"/> class from being created.
        /// </summary>
        private Forker()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Forker"/> class.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        public Forker(string projectPath)
        {
            if (RepoCollection == null)
                RepoCollection = new List<RepoItem>();

            if (StoredFolders == null)
                StoredFolders = new StringCollection();

            ProjectPath = projectPath;

            if (!Repos.ContainsKey(ProjectPath))
                Repos.Add(ProjectPath, new List<RepoItem>());
        }

        /// <summary>
        /// Adds the project path.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        public static void AddProjectPath(string projectPath)
        {
            if (!StoredFolders.Contains(projectPath))
                StoredFolders.Add(projectPath);
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
        /// Saves the instance.
        /// </summary>
        /// <exception cref="Exception">You deleted exe config file!</exception>
        public static void SaveInstance()
        {
            if (MySettings == null)
                throw new Exception("You deleted exe config file!");

            MySettings.SyncFolder = SyncFolder;

            SaveStoredFolders(false);
            SaveRepoCollection();
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
        public static void SaveRepoCollection(bool fSave = true)
        {
            MySettings.Repos = JsonConvert.SerializeObject(Repos);

            if (fSave)
                MySettings.Save();
        }
    }
}