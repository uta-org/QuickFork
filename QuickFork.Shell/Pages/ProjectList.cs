using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Common;
    using Interfaces;
    using Lib;
    using Lib.Model;

    internal class ProjectList : MenuPage, IPageList<ProjectItem>
    {
        public ProjectItem NewItem { get; set; }

        private ProjectList()
            : base("", null)
        {
        }

        public ProjectList(Program program)
            : base("Project List", program)
        {
            Instance = this;

            EmptyAction = () => NewItem = ProjectFunc.Add();
        }

        private static List<Option> GetOptions(Program program)
        {
            if (Forker.StoredProjects == null)
                return null;

            var list = ProjectFunc.Get(null).ToList();

            list.AddRange(CommonFunc.CommonOptions<ProjectItem>(program, (newProject) =>
            {
                (Instance as ProjectList).NewItem = newProject;
                CurrentProgram.NavigateBack(true, false);
            }));

            return list;
        }
    }
}
}