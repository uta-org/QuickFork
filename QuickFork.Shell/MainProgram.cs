using EasyConsole;

namespace QuickFork.Shell
{
    using Lib;
    using Pages;

    internal class MainProgram : Program
    {
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