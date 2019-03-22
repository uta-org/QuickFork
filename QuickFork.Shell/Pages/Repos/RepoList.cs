using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages.Repos
{
    using Common;
    using Interfaces;
    using Lib;
    using Lib.Model;

    /// <summary>
    /// The RepoList class (the Repositories are listed here)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    /// <seealso cref="QuickFork.Shell.Pages.Interfaces.IPageList{QuickFork.Lib.Model.RepoItem}" />
    internal sealed class RepoList : MenuPage, IPageList<RepoItem>
    {
        // Note: Not used (but needed for the interface)
        // TODO: Delete this
        /// <summary>
        /// Gets or sets the new item.
        /// </summary>
        /// <value>
        /// The new item.
        /// </value>
        public RepoItem NewItem { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="RepoList"/> class from being created.
        /// </summary>
        private RepoList()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoList"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public RepoList(Program program)
            : base("Repository List", program, () => GetOptions(program))
        {
            EmptyAction = () =>
            {
                NewItem = RepoFunc.Add();
                CurrentProgram.NavigateBack(true, PopAction.PopAfter);
            };
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="selectedRepo">The selected repo.</param>
        /// <param name="addBack">if set to <c>true</c> [add back].</param>
        /// <param name="captions">The captions.</param>
        /// <returns></returns>
        public static GetOptionsDelegate GetOptions(Program program, Action<RepoItem> selectedRepo = null, bool addBack = false, params OptionAction[] captions)
        {
            var list = Forker.StoredRepos.IsNullOrEmpty() ? new List<Option>() : RepoFunc.Get((i) => selectedRepo(Forker.StoredRepos.ElementAt(i))).ToList();

            list.AddRange(CommonFunc.CommonOptions<RepoItem>(program, DefaultAddAction, null, captions));

            if (addBack)
                list.Add(new Option("Go back", () => CurrentProgram.NavigateBack()));

            return () => list;
        }

        /// <summary>
        /// Defaults the add action.
        /// </summary>
        /// <param name="newRepo">The new repo.</param>
        private static void DefaultAddAction(RepoItem newRepo)
        {
            Forker.StoredRepos.Add(newRepo);
            CurrentProgram.NavigateBack(true, PopAction.NoPop);
        }
    }
}