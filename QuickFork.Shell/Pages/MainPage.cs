using EasyConsole;
using System;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Repos;
    using Projects;

    /// <summary>
    /// The Main Page class
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal class MainPage : MenuPage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="MainPage"/> class from being created.
        /// </summary>
        private MainPage()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public MainPage(Program program)
            : base("MainPage", program,
                  new Option("See project list", () => program.NavigateTo<ProjectList>()),
                  new Option("See repository list", () => program.NavigateTo<RepoList>()),
                  new Option("Do the project linking", () => program.NavigateTo<ProjectSelection>()),
                  new Option("Generate dependencies.json file from solution path", () => program.NavigateTo<DependenceCreator>()),
                  new Option("Exit application", () => Runner.SafeExit()))
        {
        }

        /// <summary>
        /// Displays the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
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