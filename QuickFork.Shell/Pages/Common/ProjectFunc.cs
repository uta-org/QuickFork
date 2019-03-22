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

    /// <summary>
    /// The ProjectFunc class (the main funcionality for the Projects)
    /// </summary>
    internal static class ProjectFunc
    {
        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <param name="selectedProject">The selected project.</param>
        /// <returns></returns>
        public static GetOptionsDelegate GetDelegate(Action<int> selectedProject = null)
        {
            return () => Get(selectedProject);
        }

        /// <summary>
        /// Gets the specified selected project.
        /// </summary>
        /// <param name="selectedProject">The selected project.</param>
        /// <returns></returns>
        public static IEnumerable<Option> Get(Action<int> selectedProject = null)
        {
            if (selectedProject != null && selectedProject.Equals(default(Action<int>))) selectedProject = null;
            return Forker.StoredProjects.Select((p, i) => new Option(CommonFunc.GetName(p), selectedProject == null ? (Action)null : () => selectedProject(i)));
        }

        /// <summary>
        /// Adds the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
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

                Console.WriteLine("Project has created succesfully!", Color.DarkGreen);
            }
            else
                pItem = Forker.StoredProjects.ElementAt(index);

            return pItem;
        }
    }
}