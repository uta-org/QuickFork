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

    /// <summary>
    /// The CSProjLinking (this is a mapped object where (inside Data) is stored a key representing the url and a list of string values representing the path of each *.csproj file)
    /// </summary>
    [Serializable]
    public class CsProjLinking
    {
        // TODO: Refactorize this

        [JsonProperty]
        public Dictionary<string, List<string>> Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsProjLinking"/> class.
        /// </summary>
        public CsProjLinking()
        {
            Data = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsProjLinking"/> class.
        /// </summary>
        /// <param name="gitUrl">The git URL.</param>
        /// <param name="csprojs">The csprojs.</param>
        public CsProjLinking(string gitUrl, params string[] csprojs)
            : this()
        {
            Data.AddOrAppend(gitUrl, csprojs);
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="gitUrl">The git URL.</param>
        /// <param name="csprojs">The csprojs.</param>
        public void AddLink(string gitUrl, params string[] csprojs)
        {
            //if (!
            Data.AddOrAppend(gitUrl, csprojs);
            //Console.WriteLine("The gitUrl you specified, is already available. The program has modify it!", Color.Yellow);
        }

        /// <summary>
        /// Determines whether the specified git URL has key.
        /// </summary>
        /// <param name="gitUrl">The git URL.</param>
        /// <returns>
        ///   <c>true</c> if the specified git URL has key; otherwise, <c>false</c>.
        /// </returns>
        public bool HasKey(string gitUrl)
        {
            return Data.ContainsKey(gitUrl);
        }

        /// <summary>
        /// Determines whether [has cs proj] [the specified csproj name].
        /// </summary>
        /// <param name="csprojName">Name of the csproj.</param>
        /// <returns>
        ///   <c>true</c> if [has cs proj] [the specified csproj name]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCsProj(string csprojName)
        {
            return Data.Keys.Any(csproj => csproj == csprojName);
        }

        /// <summary>
        /// Saves the dependencies.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        public void SaveDependencies(ProjectItem pItem)
        {
            SaveDependencies(pItem.GetPackageFile());
        }

        /// <summary>
        /// Saves the dependencies.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SaveDependencies(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}