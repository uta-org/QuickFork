using EasyConsole;
using System;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;

    internal class MainPage : MenuPage
    {
        private MainPage()
            : base("", null)
        {
        }

        public MainPage(Program program)
            : base("MainPage", program,
                  new Option("See project list", () => program.NavigateTo<ProjectList>()),
                  new Option("See repository list", () => program.NavigateTo<RepoList>()),
                  new Option("Do the project linking", () => program.NavigateTo<ProjectSelection>()),
                  new Option("Generate dependencies.json file from solution path", () => program.NavigateTo<DependenceCreator>()))
        {
        }

        public override void Display(string caption = "Choose an option: ")
        {
            if (string.IsNullOrEmpty(Forker.SyncFolder))
            {
                string syncPath = ConsoleHelper.GetValidPath("First of all, please, set the base folder where new repositories will be cloned: ");
                Console.WriteLine();

                Forker.SyncFolder = syncPath;
                Forker.SaveSyncFolder();
            }

            base.Display(caption);
        }
    }
}