using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using uzLib.Lite.Extensions;

namespace QuickFork.Lib
{
    using Model;

    [Serializable]
    public class Forker
    {
        private static Settings MySettings => Settings.Default;

        public static Dictionary<string, List<RepoItem>> Repos { get; private set; }

        public static StringCollection StoredFolders { get; private set; }

        [JsonProperty]
        public List<RepoItem> RepoCollection { get; private set; }

        [JsonProperty]
        public string ProjectPath { get; private set; }

        private Forker()
        {
        }

        public Forker(string projectPath)
        {
            if (RepoCollection == null)
                RepoCollection = new List<RepoItem>();

            if (StoredFolders == null)
                StoredFolders = new StringCollection();

            if (Repos == null)
                Repos = new Dictionary<string, List<RepoItem>>();

            ProjectPath = projectPath;

            if (!Repos.ContainsKey(ProjectPath))
                Repos.Add(ProjectPath, new List<RepoItem>());
        }

        public static void AddProjectPath(string projectPath)
        {
            if (!StoredFolders.Contains(projectPath))
                StoredFolders.Add(projectPath);
        }

        public static void LoadSettings()
        {
            string loadString = MySettings.Repos;

            if (!string.IsNullOrEmpty(loadString))
                Repos = JsonConvert.DeserializeObject<Dictionary<string, List<RepoItem>>>(loadString);

            if (MySettings.StoredFolders == null)
            {
                MySettings.StoredFolders = new StringCollection();
                MySettings.Save();
            }
            else
                StoredFolders = MySettings.StoredFolders;
        }

        public static void SaveInstance()
        {
            if (MySettings == null)
                throw new Exception("You deleted exe config file!");

            SaveStoredFolders(false);
            SaveRepoCollection();
        }

        public static void SaveStoredFolders(bool fSave = true)
        {
            MySettings.StoredFolders = StoredFolders;

            if (fSave)
                MySettings.Save();
        }

        public static void SaveRepoCollection(bool fSave = true)
        {
            MySettings.Repos = JsonConvert.SerializeObject(Repos);

            if (fSave)
                MySettings.Save();
        }
    }
}