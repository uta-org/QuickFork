using EasyConsole;
using System;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Common;

    internal class ProjectDeletion : MenuPage
    {
        private ProjectDeletion()
            : base("", null)
        {
        }

        public ProjectDeletion(Program program)
            : base("Project Deletion", program, RepoFunc.Get(DeleteRepo).ToArray())
        {
        }

        private static void DeleteRepo(int index)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            string projectPath = Forker.StoredProjects[index];

            Forker.StoredProjects.RemoveAt(index);
            Forker.SaveStoredProjects();

            Forker.Repos.Remove(projectPath);
            Forker.SaveRepoMap();

            CurrentProgram.NavigateBack(true);
        }
    }
}