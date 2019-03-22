using EasyConsole;

namespace QuickFork.Shell
{
    using Lib;
    using Pages;
    using Pages.Repos;
    using Pages.Projects;

    /// <summary>
    /// The Main Program class
    /// </summary>
    /// <seealso cref="EasyConsole.Program" />
    internal class MainProgram : Program
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainProgram"/> class.
        /// </summary>
        public MainProgram()
            : base("QuickFork", breadcrumbHeader: true)
        {
            Forker.LoadSettings();

            AddPage(new MainPage(this));
            AddPage(new DependenceCreator(this));
            AddPage(new ProjectList(this));
            AddPage(new RepoList(this));
            AddPage(new ProjectSelection(this));

            SetPage<MainPage>();
        }
    }
}