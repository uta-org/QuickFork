using System;
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

            AddPage(new MainPage(this,
                new Option("Fork Syncing (complete process)", () =>
                {
                    ForkSyncing.DoLinking = null;
                    NavigateTo<ForkSyncing>();
                }),
                new Option("Fork Syncing (only cloning)", () =>
                {
                    ForkSyncing.DoLinking = true;
                    NavigateTo<ForkSyncing>();
                }),
                new Option("Fork Syncing (only linking)", () =>
                {
                    ForkSyncing.DoLinking = false;
                    NavigateTo<ForkSyncing>();
                }),
                new Option("Exit", () => Environment.Exit(0))));

            //AddPage(new ForkSyncing(this));

            SetPage<MainPage>();
        }
    }
}