using System;
using System.Collections.Generic;
using System.Linq;
using EasyConsole;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;

    internal static class ProjectFunc
    {
        public static IEnumerable<Option> Get(Action<int> selectedProject)
        {
            return Forker.StoredProjects?.Cast<string>().Select((r, i) => new Option(r, () => selectedProject?.Invoke(i)));
        }

        public static ProjectItem Add()
        {
            return null;
        }
    }
}