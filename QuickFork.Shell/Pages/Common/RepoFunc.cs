using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;

    internal static class RepoFunc
    {
        public static IEnumerable<Option> Get(ProjectItem item, Action<int, ProjectItem> action)
        {
            if (Forker.Repos.Count > 0 && Forker.Repos.ContainsKey(item.SelectedPath))
                return Forker.Repos[item.SelectedPath].AsEnumerable().Select((r, i) => new Option(r.ToString(), () => action?.Invoke(i, item)));
            else
                return null;
        }
    }
}