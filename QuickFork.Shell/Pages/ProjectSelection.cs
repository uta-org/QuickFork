using EasyConsole;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using System;
    using System.Drawing;

    using Console = Colorful.Console;

    internal sealed class ProjectSelection : MenuPage
    {
        private ProjectSelection()
            : base("", null, null)
        {
        }

        public ProjectSelection(Program program)
            : base("Project Selection", program, GetOptions().ToArray())
        {
        }

        private static List<Option> GetOptions()
        {
            List<Option> list = new List<Option>();

            if (Forker.StoredProjects.Count > 0)
                Forker.StoredProjects.Cast<string>().ForEach((path, i) => list.Add(new Option(Path.GetFileName(path), () => SelectProject(i))));

            list.Add(new Option("Add new project", () => SelectProject(-1)));
            list.Add(new Option("Exit", () => Environment.Exit(0)));

            return list;
        }

        private static void SelectProject(int index)
        {
            string projectPath;

            if (index == -1)
            {
                projectPath = ConsoleHelper.GetValidPath("Write the path to your project: ");
                Console.WriteLine();

                Console.WriteLine("Project has created succesfully!", Color.Green);
            }
            else
                projectPath = Forker.StoredProjects[index];

            CurrentProgram.AddPage(new ProjectOperation(CurrentProgram, new ProjectItem(projectPath)));
            CurrentProgram.NavigateTo<ProjectOperation>();
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.StoredProjects.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available project to select, please, create a new one.", Color.LightBlue);
                Console.WriteLine();
                SelectProject(-1);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.StoredProjects.Count} local projects available. Which do you wish to use?");
                Console.WriteLine();

                base.Display(caption);
            }
        }
    }
}