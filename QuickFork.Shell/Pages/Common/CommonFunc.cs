using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;
    using Lib.Model.Interfaces;

    internal static class CommonFunc
    {
        public static IEnumerable<Option> CommonOptions<T>(Program program, Action<T> addAction, ProjectItem pItem = null, params OptionAction[] captions)
            where T : IModel
        {
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

        internal static string GetName(IModel element)
        {
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

        private static IModel Add(bool fIsRepo)
        {
            if (fIsRepo)
                return RepoFunc.Add();
            else
                return ProjectFunc.Add();
        }

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