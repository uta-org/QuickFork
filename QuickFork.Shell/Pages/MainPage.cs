using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;

    internal class MainPage : MenuPage
    {
        private MainPage()
            : base("", null, null)
        {
        }

        public MainPage(Program program)
            : base("MainPage", program,
                  new Option("See project list", () => program.NavigateTo<ProjectList>()),
                  new Option("See repository list", () => program.NavigateTo<RepoList>()),
                  new Option("Do the project linking", () => program.NavigateTo<ProjectSelection>()))
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