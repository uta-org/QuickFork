using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages.Projects
{
    using Common;
    using Interfaces;
    using Lib;
    using Lib.Model;

    /// <summary>
    /// The ProjectList class (all the Projects are listed here)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    /// <seealso cref="QuickFork.Shell.Pages.Interfaces.IPageList{QuickFork.Lib.Model.ProjectItem}" />
    internal class ProjectList : MenuPage, IPageList<ProjectItem>
    {
        // Note: Not used (but needed for the interface)
        // TODO: Delete this
        public ProjectItem NewItem { get; set; }

        private ProjectList()
            : base("", null)
        {
        }

        public ProjectList(Program program)
            : base("Project List", program, () => GetOptions(program))
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