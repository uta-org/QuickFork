using EasyConsole;
using System;

namespace QuickFork.Shell
{
    using Pages;
    using Lib.Properties;
    using uzLib.Lite.Extensions;

    internal class MainPage : MenuPage
    {
        private static Settings LibSettings => Settings.Default;

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

        public override void Display()
        {
            if (string.IsNullOrEmpty(LibSettings.SyncFolder))
            {
                string syncPath = ConsoleHelper.GetValidPath("First of all, please, set the base folder where new repositories will be cloned: ");

                LibSettings.SyncFolder = syncPath;
                LibSettings.Save();
            }

            base.Display();
        }
    }
}