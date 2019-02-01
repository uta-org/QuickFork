using System;
using System.IO;
using Newtonsoft.Json;

namespace QuickFork.Lib.Model
{
    using Interfaces;

    [Serializable]
    public class ProjectItem : IModel
    {
        [JsonProperty]
        public string SelectedPath { get; private set; }

        [JsonIgnore]
        public string Name => Path.GetFileName(SelectedPath);

        [JsonIgnore]
        public OperationType Type { get; set; }

        private ProjectItem()
        {
        }

        public ProjectItem(string selectedPath)
        {
            SelectedPath = selectedPath;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}