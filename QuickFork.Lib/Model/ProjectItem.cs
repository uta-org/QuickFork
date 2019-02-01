using System.IO;

namespace QuickFork.Lib.Model
{
    using Interfaces;

    public class ProjectItem : IModel
    {
        public string SelectedPath { get; private set; }

        public string Name => Path.GetFileName(SelectedPath);

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