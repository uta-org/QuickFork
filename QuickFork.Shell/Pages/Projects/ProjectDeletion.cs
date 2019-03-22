using EasyConsole;
using System;
using System.Linq;

namespace QuickFork.Shell.Pages.Projects
{
    using Common;
    using Lib;
    using Lib.Model;

    /// <summary>
    /// The ProjectDeletion class (the Projects are deleted here)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal class ProjectDeletion : MenuPage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ProjectDeletion"/> class from being created.
        /// </summary>
        private ProjectDeletion()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDeletion"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public ProjectDeletion(Program program)
            : base("Project Deletion", program, () => ProjectFunc.GetDelegate(DeleteProject))
        {
        }

        /// <summary>
        /// Deletes the project.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="ArgumentException">index - Index cannot be null.</exception>
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