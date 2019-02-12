using EasyConsole;

using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    internal class DependenceCreator : MenuPage
    {
        private DependenceCreator()
            : base("", null)
        {
        }

        public DependenceCreator(Program program)
            : base("Dependence Creator", program, () => GetOptions(program))
        {
        }

        // Implement here a list of projects (like ProjectList) + add/remove, and then, when you select any project, then you only get ProjectItem and call with this to RepoItem.CreateDependencies(pItem)
        private static GetOptionsDelegate GetOptions(Program program)
        {
            var list = Forker.StoredProjects == null ? new List<Option>() : ProjectFunc.Get((index) => Forker.StoredProjects.ElementAt(index).CreateDependencies()).ToList();

            list.AddRange(CommonFunc.CommonOptions<ProjectItem>(program, (newProject) =>
            {
                CurrentProgram.NavigateBack(true, PopAction.NoPop);
            }));

            return () => list;
        }
    }
}