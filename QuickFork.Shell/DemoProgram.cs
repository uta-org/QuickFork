using System;
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

            AddPage(new ForkSyncing(this));

            SetPage<MainPage>();
        }
    }
}