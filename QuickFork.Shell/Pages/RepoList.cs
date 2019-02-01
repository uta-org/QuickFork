using System;
using EasyConsole;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;
    using Interfaces;

    internal sealed class RepoList : MenuPage, IPageList<RepoItem>
    {
        public RepoItem NewItem { get; set; }

        private RepoList()
            : base("", null)
        {
        }

        public RepoList(Program program)
            : base("Repository List", program, GetOptions(program))
        {
            Instance = this;

            EmptyAction = () => NewItem = RepoFunc.Add();
        }

        private static Func<IEnumerable<Option>> GetOptions(Program program)
        {
            if (Forker.StoredRepos.IsNullOrEmpty())
                return null;

            var list = RepoFunc.Get(null).ToList();

            list.AddRange(CommonFunc.CommonOptions<RepoItem>(program, (newRepo) =>
            {
                (Instance as RepoList).NewItem = newRepo;
                CurrentProgram.NavigateBack(true, false);
            }));

            return () => list;
        }
    }
}