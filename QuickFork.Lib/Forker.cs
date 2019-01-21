using Newtonsoft.Json;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using uzLib.Lite.Extensions;

namespace QuickFork.Lib
{
    public static class QuickFork
    {
        private static Settings MySettings => Settings.Default;

        public static List<RepoItem> RepoCollection { get; private set; }

        public static RepoItem Fork(string gitUrl, string folderPath)
        {
            return RepoCollection.InsertOrGet(new RepoItem(folderPath, gitUrl), r => r.GitUrl == gitUrl);
        }

        public static void LoadSettings()
        {
            RepoCollection = JsonConvert.DeserializeObject<List<RepoItem>>(MySettings.RepoCollection);
        }

        public static void SaveInstance()
        {
            if (MySettings == null)
                throw new Exception("You deleted exe config file!");

            MySettings.RepoCollection = JsonConvert.SerializeObject(RepoCollection);
            MySettings.Save();
        }
    }
}