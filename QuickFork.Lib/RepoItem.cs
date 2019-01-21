using System.IO;
using uzLib.Lite;

namespace QuickFork.Lib
{
    public class RepoItem
    {
        public static GitShell MyShell { get; private set; }
        public string FolderPath { get; private set; }
        public string GitUrl { get; private set; }

        private RepoItem()
        {
            MyShell = new GitShell();
        }

        public RepoItem(string folderPath, string gitUrl)
            : this()
        {
            FolderPath = folderPath;
            GitUrl = gitUrl;
        }

        public async void Execute(string projectPath, OperationType operationType = OperationType.AddProjToSLN)
        {
            string folderName = Path.GetFileName(FolderPath);

            MyShell.CurrentInfo.WorkingDirectory = Path.GetDirectoryName(FolderPath);
            await MyShell.SendCommand($"clone {GitUrl} {folderName}");

            switch (operationType)
            {
                case OperationType.AddProjToSLN:
                    break;

                case OperationType.CreateSymlink:
                    NativeMethods.CreateSymbolicLink(FolderPath, folderName, NativeEnums.SymbolicLinkFlags.Directory);
                    break;
            }
        }
    }
}