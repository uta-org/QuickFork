using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    internal class ProjectOperation : MenuPage
    {
        private static ProjectItem CurrentItem { get; set; }

        private ProjectOperation()
            : base("", null, null)
        {
        }

        public ProjectOperation(Program program, ProjectItem item)
            : base("Project Operation", program, GetOptions().ToArray())
        {
            CurrentItem = item;
        }

        public static IEnumerable<Option> GetOptions()
        {
            yield return new Option("Search .csproj and add to solution specified", () => Set(OperationType.AddProjToSLN));
            yield return new Option("Create a symlink", () => Set(OperationType.CreateSymlink));
        }

        private static void Set(OperationType type)
        {
            CurrentItem.Type = type;

            MainProgram.Instance.AddPage(new RepoSelection(MainProgram.Instance, CurrentItem));
            MainProgram.Instance.NavigateTo<RepoSelection>();
        }
    }
}