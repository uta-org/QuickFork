using EasyConsole;
using System;
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
            : base("Repository List", program, () => GetOptions(program))
        {
            EmptyAction = () =>
            {
                NewItem = RepoFunc.Add();
                CurrentProgram.NavigateBack(true, PopAction.PopAfter);
            };
        }

        public static GetOptionsDelegate GetOptions(Program program, Action<RepoItem> selectedRepo = null, bool addBack = false, params OptionAction[] captions)
        {
            var list = Forker.StoredRepos.IsNullOrEmpty() ? new List<Option>() : RepoFunc.Get((i) => selectedRepo(Forker.StoredRepos.ElementAt(i))).ToList();

            list.AddRange(CommonFunc.CommonOptions<RepoItem>(program, (newRepo) =>
            {
                CurrentProgram.NavigateBack(true, PopAction.NoPop);
            }, null, captions));

            if (addBack)
                list.Add(new Option("Go back", () => CurrentProgram.NavigateBack()));

            return () => list;
        }
    }
}