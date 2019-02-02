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
        // Not used
        public ProjectItem NewItem { get; set; }

        private ProjectList()
            : base("", null)
        {
        }

        public ProjectList(Program program)
            : base("Project List", program, (_p) => GetOptions(_p))
        {
            EmptyAction = () =>
            {
                NewItem = ProjectFunc.Add();
                CurrentProgram.NavigateBack(true, PopAction.PopAfter);
            };
        }

        private static GetOptionsDelegate GetOptions(Program program)
        {
            var list = Forker.StoredProjects == null ? new List<Option>() : ProjectFunc.Get().ToList();

            list.AddRange(CommonFunc.CommonOptions<ProjectItem>(program, (newProject) =>
            {
                CurrentProgram.NavigateBack(true, PopAction.NoPop);
                (Instance as ProjectList).NewItem = newProject;
            }));

            return () => list;
        }
    }
}