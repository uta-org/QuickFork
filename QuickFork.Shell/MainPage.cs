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
          new Option("Fork Syncing (complete process)", () => program.NavigateTo<ForkSyncing>().DoLinking = null),
          new Option("Fork Syncing (only cloning)", () => program.NavigateTo<ForkSyncing>().DoLinking = true),
          new Option("Fork Syncing (only linking)", () => program.NavigateTo<ForkSyncing>().DoLinking = false),
          new Option("Exit", () => Environment.Exit(0)))
        {
        }
    }
}