using System.Linq;
using EasyConsole;

namespace QuickFork.Shell
{
    using Pages;
    using Lib;

    internal class MainProgram : Program
    {
        public static MainProgram Instance { get; private set; }

        public MainProgram()
            : base("QuickFork", breadcrumbHeader: true)
        {
            Instance = this;

            Forker.LoadSettings();

            AddPage(new ProjectSelection(this));

            SetPage<ProjectSelection>();
        }
    }
}