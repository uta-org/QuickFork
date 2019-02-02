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
        // Not used
        public RepoItem NewItem { get; set; }

        private RepoList()
            : base("", null)
        {
        }

        public RepoList(Program program)
            : base("Repository List", program, (_p) => GetOptions(_p))
        {
            EmptyAction = () =>
            {
                NewItem = RepoFunc.Add();
                CurrentProgram.NavigateBack(true, PopAction.PopAfter);
            };
        }

        private static GetOptionsDelegate GetOptions(Program program)
        {
            var list = Forker.StoredRepos.IsNullOrEmpty() ? new List<Option>() : RepoFunc.Get().ToList();

            list.AddRange(CommonFunc.CommonOptions<RepoItem>(program, (newRepo) =>
            {
                CurrentProgram.NavigateBack(true, PopAction.NoPop);
                (Instance as RepoList).NewItem = newRepo;
            }));

            return () => list;
        }
    }
}