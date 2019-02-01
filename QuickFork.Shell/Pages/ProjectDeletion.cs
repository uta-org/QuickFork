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
            : base("Project Deletion", program, RepoFunc.GetRepoList(DeleteRepo).ToArray())
        {
        }

        private static void DeleteRepo(int index)
        {
            if (index < 0)
                throw new ArgumentException("index", "Index cannot be null.");

            Forker.StoredProjects.RemoveAt(index);
            Forker.SaveStoredProjects();

            CurrentProgram.NavigateBack(true);
        }
    }
}