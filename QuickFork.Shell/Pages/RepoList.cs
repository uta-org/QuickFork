using EasyConsole;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    internal class RepoList : MenuPage
    {
        private RepoItem NewRepoItem { get; set; }

        private RepoList()
            : base("", null)
        {
        }

        public RepoList(Program program)
            : base("Repository List", program, () => GetOptions(program).ToArray())
        {
            Instance = this;

            EmptyAction = () => NewRepoItem = RepoFunc.RepoAdd();
        }

        private static List<Option> GetOptions(Program program)
        {
            if (Forker.StoredRepos.IsNullOrEmpty())
                return null;

            var list = RepoFunc.GetRepoList(null).ToList();

            list.AddRange(RepoFunc.CommonRepoOptions(program, (newRepo) =>
            {
                (Instance as RepoList).NewRepoItem = newRepo;
                CurrentProgram.NavigateBack(true, false);
            }));

            return list;
        }
    }
}