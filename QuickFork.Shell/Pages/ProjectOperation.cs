using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    internal sealed class ProjectOperation : MenuPage
    {
        private static ProjectItem CurrentItem { get; set; }

        private ProjectOperation()
            : base("", null)
        {
        }

        public ProjectOperation(Program program, ProjectItem item)
            : base("Project Operation", program, GetOptions().ToArray())
        {
            CurrentItem = item;

            // Save it (in case of exception this will saved before exception occurs)
            Forker.Add(item.SelectedPath);
        }

        public static IEnumerable<Option> GetOptions()
        {
            yield return new Option("Search .csproj and add to solution specified", () => Set(OperationType.AddProjToSLN));
            yield return new Option("Create a symlink", () => Set(OperationType.CreateSymlink));
        }

        private static void Set(OperationType type)
        {
            CurrentItem.Type = type;

            CurrentProgram.AddPage(new RepoSelection(CurrentProgram, CurrentItem));
            CurrentProgram.NavigateTo<RepoSelection>();
        }
    }
}