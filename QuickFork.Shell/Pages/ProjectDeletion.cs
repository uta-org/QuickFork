using EasyConsole;
using System;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    internal class ProjectDeletion : MenuPage
    {
        private ProjectDeletion()
            : base("", null)
        {
        }

        public ProjectDeletion(Program program)
            : base("Project Deletion", program, (_p) => ProjectFunc.Get(_p, DeleteProject))
        {
        }

        private static void DeleteProject(int index)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            ProjectItem pItem = Forker.StoredProjects.ElementAt(index);

            Forker.Repos.Remove(pItem.SelectedPath);
            Forker.SaveRepoMap();

            Forker.StoredProjects.Remove(pItem);
            Forker.SaveStoredProjects();

            CurrentProgram.NavigateBack(true);
        }
    }
}