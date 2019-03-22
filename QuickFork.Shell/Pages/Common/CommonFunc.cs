using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;
    using Lib.Model.Interfaces;
    using Repos;
    using Projects;

    /// <summary>
    /// The CommonFunc class (here are some of the common methods for Repositories and Projects)
    /// </summary>
    internal static class CommonFunc
    {
        /// <summary>
        /// Commons the options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="program">The program.</param>
        /// <param name="addAction">The add action.</param>
        /// <param name="pItem">The p item.</param>
        /// <param name="captions">The captions.</param>
        /// <returns></returns>
        public static IEnumerable<Option> CommonOptions<T>(Program program, Action<T> addAction, ProjectItem pItem = null, params OptionAction[] captions)
            where T : IModel
        {
            // TODO: Refactorize this

            bool isActionReady = captions != null && captions.Length == 2;

            bool isRepo = typeof(RepoItem) == typeof(T);

            yield return new Option(isActionReady && captions[0] != null ? captions[0].Caption : (isRepo ? "Create new local cloned repository" : "Add new project"),
                () => addAction?.Invoke((T)Add(isRepo)));
            yield return new Option(isActionReady && captions[1] != null ? captions[1].Caption : $"Remove {(isRepo ? "repository" : "project")} from the list",
                isActionReady && captions[1] != null ? captions[1].Action : () =>
            {
                program.AddPage(isRepo ? (pItem == null ? (Page)new RepoDeletion(program) : new RepoDeletion(program, pItem)) : new ProjectDeletion(program));

                if (isRepo)
                    program.NavigateTo<RepoDeletion>();
                else
                    program.NavigateTo<ProjectDeletion>();
            });
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        internal static string GetName(IModel element)
        {
            // TODO: Refactorize this

            string retValue = element.Name;

            if (element is RepoItem)
            {
                RepoItem rItem = element as RepoItem;

                if (Forker.StoredRepos.Any(r => r.Name == element.Name))
                    retValue += $" ({rItem.GitUrl.SafeTruncate()})";
            }
            else if (element is ProjectItem)
            {
                ProjectItem pItem = element as ProjectItem;

                if (Forker.StoredProjects.Any(p => p.Name == element.Name))
                    retValue += $" ({pItem.SelectedPath.SafeTruncate()})";
            }

            return retValue;
        }

        /// <summary>
        /// Adds the specified f is repo.
        /// </summary>
        /// <param name="fIsRepo">if set to <c>true</c> [f is repo].</param>
        /// <returns></returns>
        private static IModel Add(bool fIsRepo)
        {
            if (fIsRepo)
                return RepoFunc.Add();
            else
                return ProjectFunc.Add();
        }

        /// <summary>
        /// Safes the truncate.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="maxLen">The maximum length.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns></returns>
        private static string SafeTruncate(this string str, int maxLen = 30, int threshold = 10)
        {
            if (str.Length < maxLen)
                return str;
            else if (str.Length < maxLen + threshold)
                return "..." + str.Substring(threshold);
            else
                return "..." + str.Substring(maxLen);
        }
    }
}