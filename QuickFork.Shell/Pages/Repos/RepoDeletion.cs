using EasyConsole;
using System;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages.Repos
{
    using Common;
    using Lib;
    using Lib.Model;

    /// <summary>
    /// The RepoDeletion class (the Repositories are deleted here)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal sealed class RepoDeletion : MenuPage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="RepoDeletion"/> class from being created.
        /// </summary>
        private RepoDeletion()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoDeletion"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="item">The item.</param>
        public RepoDeletion(Program program, ProjectItem item)
            : base("Repository Deletion", program, () => RepoFunc.GetDelegate(item, (i, _item) => DeleteRepo(i, _item)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoDeletion"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public RepoDeletion(Program program)
            : base("Repository Deletion", program, () => RepoFunc.GetDelegate(DeleteRepo))
        {
        }

        /// <summary>
        /// Deletes the repo.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentException">index - Index cannot be null.</exception>
        private static void DeleteRepo(int index, ProjectItem item)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            Forker.Repos[item.SelectedPath].RemoveAt(index);
            Forker.DoRemapping();
            Forker.SaveRepoMap();

            CurrentProgram.NavigateBack(true);
        }

        /// <summary>
        /// Deletes the repo.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="ArgumentException">index - Index cannot be null.</exception>
        private static void DeleteRepo(int index)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            RepoItem rItem = Forker.StoredRepos.ElementAt(index);

            Forker.Repos.ForEach(r => r.Value.Remove(rItem));
            Forker.DoRemapping(); // You must do the Repos -> RepoMap mapping before saving anything (we could even add an enum to do nothing or doing the remapping (Repos -> RepoMap or RepoMap -> Repos))
            Forker.SaveRepoMap();

            Forker.StoredRepos.Remove(rItem);
            Forker.SaveStoredRepos();

            CurrentProgram.NavigateBack(true);
        }
    }
}