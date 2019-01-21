using EasyConsole;
using System;

namespace QuickFork.Shell
{
    using Pages;

    internal class MainPage : MenuPage
    {
        private MainPage()
            : base("", null, null)
        {
        }

        public MainPage(Program program)
            : base("Main Page", program,
          new Option("Fork Syncing", () => program.NavigateTo<ForkSyncing>()),
          new Option("Exit", () => Environment.Exit(0)))
        {
        }
    }
}