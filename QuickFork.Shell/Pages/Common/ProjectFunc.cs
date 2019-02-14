using EasyConsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using uzLib.Lite.Extensions;
using Console = Colorful.Console;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;

    internal static class ProjectFunc
    {
        public static GetOptionsDelegate GetDelegate(Action<int> selectedProject = null)
        {
            return () => Get(selectedProject);
        }

        public static IEnumerable<Option> Get(Action<int> selectedProject = null)
        {
            if (selectedProject != null && selectedProject.Equals(default(Action<int>))) selectedProject = null;
            return Forker.StoredProjects.Select((p, i) => new Option(CommonFunc.GetName(p), selectedProject == null ? (Action)null : () => selectedProject(i)));
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