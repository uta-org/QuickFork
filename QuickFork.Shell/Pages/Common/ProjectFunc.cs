using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using EasyConsole;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;
using static EasyConsole.MenuPage;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;

    internal static class ProjectFunc
    {
        public static GetOptionsDelegate Get(Program program, Action<int> selectedProject)
        {
            return () => Get(selectedProject);
        }

        public static IEnumerable<Option> Get(Action<int> selectedProject = null)
        {
            if (selectedProject != null && selectedProject.Equals(default(Action<int>))) selectedProject = null;
            return Forker.StoredProjects.Select((r, i) => new Option(r.Name, selectedProject == null ? (Action)null : () => selectedProject(i)));
        }

        public static ProjectItem Add(int index = -1)
        {
            ProjectItem pItem;

            if (index == -1)
            {
                string projectPath = ConsoleHelper.GetValidPath("Write the path to your project: ");
                pItem = new ProjectItem(projectPath);

                Console.WriteLine();

                // Save it (in case of exception this will saved before exception occurs)
                Forker.Add(pItem);

                Console.WriteLine("Project has created succesfully!", Color.Green);
            }
            else
                pItem = Forker.StoredProjects.ElementAt(index);

            return pItem;
        }
    }
}