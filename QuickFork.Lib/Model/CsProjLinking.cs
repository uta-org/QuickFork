using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// using System.Drawing;

using uzLib.Lite.Extensions;

namespace QuickFork.Lib.Model
{
    // using Console = Colorful.Console;

    [Serializable]
    public class CsProjLinking
    {
        [JsonProperty]
        public Dictionary<string, List<string>> Data { get; private set; }

        public CsProjLinking()
        {
            Data = new Dictionary<string, List<string>>();
        }

        public CsProjLinking(string gitUrl, params string[] csprojs)
            : this()
        {
            Data.AddOrAppend(gitUrl, csprojs);
        }

        public void AddLink(string gitUrl, params string[] csprojs)
        {
            //if (!
            Data.AddOrAppend(gitUrl, csprojs);
            //Console.WriteLine("The gitUrl you specified, is already available. The program has modify it!", Color.Yellow);
        }

        public bool HasKey(string gitUrl)
        {
            return Data.ContainsKey(gitUrl);
        }

        public bool HasCsProj(string csprojName)
        {
            return Data.Keys.Any(csproj => csproj == csprojName);
        }

        public void SaveDependencies(ProjectItem pItem)
        {
            SaveDependencies(pItem.GetPackageFile());
        }

        public void SaveDependencies(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}