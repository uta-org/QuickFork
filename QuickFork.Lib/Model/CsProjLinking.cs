using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Lib.Model
{
    using Console = Colorful.Console;

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
    }
}