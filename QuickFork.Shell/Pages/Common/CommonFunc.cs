using EasyConsole;
using System;
using System.Collections.Generic;

namespace QuickFork.Shell.Pages.Common
{
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

        private static IModel Add(bool fIsRepo)
        {
            if (fIsRepo)
                return RepoFunc.Add();
            else
                return ProjectFunc.Add();
        }
    }
}