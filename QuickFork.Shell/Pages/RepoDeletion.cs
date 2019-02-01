using EasyConsole;
using System;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    internal sealed class RepoDeletion : MenuPage
    {
        private RepoDeletion()
            : base("", null)
        {
        }

        public RepoDeletion(Program program, ProjectItem item)
            : base("Repository Deletion", program, () => RepoFunc.Get(item, (i, _item) => DeleteRepo(i, _item)).ToArray())
        {
        }

        public RepoDeletion(Program program)
            : base("Repository Deletion", program, () => RepoFunc.Get(DeleteRepo).ToArray())
        {
        }

        private static void DeleteRepo(int index, ProjectItem item)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            Forker.Repos[item.SelectedPath].RemoveAt(index);
            Forker.SaveRepoMap();

            CurrentProgram.NavigateBack();
        }

        private static void DeleteRepo(int index)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            RepoItem rItem = Forker.StoredRepos.ElementAt(index);

            Forker.Repos.ForEach(r => r.Value.Remove(rItem));
            Forker.SaveRepoMap();

            Forker.StoredRepos.Remove(rItem);
            Forker.SaveStoredRepos();

            CurrentProgram.NavigateBack(true);
        }
    }
}