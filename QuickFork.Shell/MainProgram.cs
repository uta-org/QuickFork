﻿using System.Linq;
using EasyConsole;

namespace QuickFork.Shell
{
    using Pages;
    using Lib;

    internal class MainProgram : Program
    {
        public MainProgram()
            : base("QuickFork", breadcrumbHeader: true)
        {
            Instance = this;

            Forker.LoadSettings();

            AddPage(new MainPage(this));
            AddPage(new ProjectList(this));
            AddPage(new RepoList(this));
            AddPage(new ProjectSelection(this));

            SetPage<MainPage>();
        }
    }
}