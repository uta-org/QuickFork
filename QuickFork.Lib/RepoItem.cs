using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using System;
using System.IO;
using System.Linq;
using uzLib.Lite;
using uzLib.Lite.Extensions;

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
                    string[] solutions = Directory.GetFiles(projectPath, "*.sln", SearchOption.AllDirectories);

                    if (solutions.Length == 0)
                    {
                        Console.WriteLine("There is any solution available yet!");
                        return;
                    }
                    else if (solutions.Length > 1)
                    {
                        Console.WriteLine("Multiple solutions isn't supported yet!");
                        return;
                    }

                    var solution = SolutionParser.Parse(solutions[0]) as Solution;
                    string[] projs = Directory.GetFiles(FolderPath, "*.csproj", SearchOption.AllDirectories);

                    if (projs.Length == 0)
                    {
                        Console.WriteLine($"The cloned repo '{Path.GetFileName(FolderPath)}' doesn't have any *.csproj files. If you need to support another kind of project, please, fork this project and implement it!");
                        return;
                    }
                    else if (projs.Length == 1)
                    {
                        var guid = Guid.NewGuid();
                        solution.Projects = solution.Projects.ToList().AddAndGet(new Project(guid, Path.GetFileNameWithoutExtension(projectPath), projectPath, guid));

                        File.WriteAllText(solutions[0], SolutionRenderer.Render(solution));
                    }
                    else
                    {
                        // Let user choose one, several or all csprojs.

                        Console.WriteLine($"Multiple projects option aren't implemented yet.");
                        return;
                    }

                    break;

                case OperationType.CreateSymlink:
                    NativeMethods.CreateSymbolicLink(FolderPath, folderName, NativeEnums.SymbolicLinkFlags.Directory);
                    break;
            }
        }

        public override string ToString()
        {
            return $"{GitUrl.GetFileNameFromUrl()} - {Path.GetFileName(FolderPath)}";
        }
    }
}