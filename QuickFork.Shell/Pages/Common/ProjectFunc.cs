using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using EasyConsole;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;

    internal static class ProjectFunc
    {
        public static IEnumerable<Option> Get(Action<int> selectedProject = null)
        {
            return Forker.StoredProjects?.Cast<string>().Select((r, i) => new Option(r, selectedProject == null ? (Action)(() => { }) : () => selectedProject(i)));
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