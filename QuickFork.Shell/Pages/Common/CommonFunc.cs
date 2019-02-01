using System;
using System.Collections.Generic;
using EasyConsole;

namespace QuickFork.Shell.Pages.Common
{
    using Lib.Model;
    using Lib.Model.Interfaces;

    internal static class CommonFunc
    {
        public static IEnumerable<Option> CommonOptions<T>(Program program, Action<T> addAction)
            where T : IModel
        {
            bool isRepo = typeof(RepoItem) == typeof(T);

            yield return new Option(isRepo ? "Create new local cloned repository" : "Add new project", () => addAction?.Invoke((T)Add(isRepo)));
            yield return new Option($"Remove {(isRepo ? "repository" : "project")} from the list", () =>
            {
                program.AddPage(isRepo ? (Page)new RepoDeletion(program) : new ProjectDeletion(program));

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