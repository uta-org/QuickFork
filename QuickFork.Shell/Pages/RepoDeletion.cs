using EasyConsole;
using System;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    internal sealed class RepoDeletion : MenuPage
    {
        private RepoDeletion()
            : base("", null, null)
        {
        }

        public RepoDeletion(Program program, ProjectItem item)
            : base("Repository Selection", program, RepoFunc.Get(item, (i, _item) => DeleteRepo(i, _item)).ToArray())
        {
        }

        private static void DeleteRepo(int index, ProjectItem item)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            Forker.Repos[item.SelectedPath].RemoveAt(index);
            Forker.SaveRepos();

            CurrentProgram.NavigateBack();
        }
    }
}