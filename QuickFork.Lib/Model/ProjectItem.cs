namespace QuickFork.Lib.Model
{
    public class ProjectItem
    {
        public string SelectedPath { get; private set; }

        public OperationType Type { get; set; }

        private ProjectItem()
        {
        }

        public ProjectItem(string selectedPath)
        {
            SelectedPath = selectedPath;
        }
    }
}