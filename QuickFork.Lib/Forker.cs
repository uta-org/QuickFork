using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using uzLib.Lite.Extensions;

namespace QuickFork.Lib
{
    public static class Forker
    {
        private static Settings MySettings => Settings.Default;

        public static List<RepoItem> RepoCollection { get; private set; }
        public static StringCollection StoredFolders { get; private set; }

        static Forker()
        {
            if (RepoCollection == null)
                RepoCollection = new List<RepoItem>();
        }

        public static RepoItem Fork(string gitUrl, bool fSave = true)
        {
            bool firstTime;
            var item = RepoCollection.InsertOrGet(new RepoItem(gitUrl), r => r.GitUrl == gitUrl, out firstTime);

            if (fSave && firstTime)
                SaveInstance();

            return item;
        }

        public static void AddProjectPath(string projectPath)
        {
            if (!StoredFolders.Contains(projectPath))
                StoredFolders.Add(projectPath);
        }

        public static void LoadSettings()
        {
            string loadString = MySettings.RepoCollection;

            if (!string.IsNullOrEmpty(loadString))
                RepoCollection = JsonConvert.DeserializeObject<List<RepoItem>>(loadString);
        }

        public static void SaveInstance()
        {
            if (MySettings == null)
                throw new Exception("You deleted exe config file!");

            MySettings.RepoCollection = JsonConvert.SerializeObject(RepoCollection);
            MySettings.StoredFolders = StoredFolders;
            MySettings.Save();
        }
    }
}