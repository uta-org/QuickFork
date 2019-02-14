﻿using EasyConsole;
using System;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Common;
    using Lib;
    using Lib.Model;

    internal class ProjectDeletion : MenuPage
    {
        private ProjectDeletion()
            : base("", null)
        {
        }

        public ProjectDeletion(Program program)
            : base("Project Deletion", program, () => ProjectFunc.GetDelegate(DeleteProject))
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