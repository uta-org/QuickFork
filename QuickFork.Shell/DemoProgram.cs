using EasyConsole;

namespace QuickFork.Shell
{
    using Pages;
    using Lib;

    internal class DemoProgram : Program
    {
        public DemoProgram()
            : base("EasyConsole Demo", breadcrumbHeader: true)
        {
            Forker.LoadSettings();

            AddPage(new MainPage(this));
            AddPage(new ForkSyncing(this));

            SetPage<MainPage>();
        }
    }
}