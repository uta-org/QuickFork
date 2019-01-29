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

    internal class ProjectSelection : MenuPage
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

            if (Forker.StoredFolders.Count > 0)
                Forker.StoredFolders.Cast<string>().ForEach((path, i) => list.Add(new Option(Path.GetFileName(path), () => SelectProject(i))));

            list.Add(new Option("Add new project", () => SelectProject(-1)));
            list.Add(new Option("Exit", () => Environment.Exit(0)));

            return list;
        }

        private static void SelectProject(int index)
        {
            string projectPath;

            if (index == -1)
            {
                Console.WriteLine();
                projectPath = ConsoleHelper.GetValidPath("Write the path to your project: ");
                Console.WriteLine();

                Forker.StoredFolders.Add(projectPath);
                Forker.SaveStoredFolders();

                Console.WriteLine("Project has created succesfully!");
            }
            else
                projectPath = Forker.StoredFolders[index];

            MainProgram.Instance.AddPage(new ProjectOperation(MainProgram.Instance, new ProjectItem(projectPath)));
            MainProgram.Instance.NavigateTo<ProjectOperation>();
        }

        public override void Display()
        {
            if (string.IsNullOrEmpty(Forker.SyncFolder))
            {
                string syncPath = ConsoleHelper.GetValidPath("First of all, please, set the base folder where new repositories will be cloned: ");
                Console.WriteLine();

                Forker.SyncFolder = syncPath;
                Forker.SaveInstance();
            }

            bool isNew = Forker.StoredFolders.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available project to select, please, create a new one.");
                Console.WriteLine();
                SelectProject(-1);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.StoredFolders.Count} local projects available. Which do you wish to use?");
                Console.WriteLine();

                base.Display();
            }
        }
    }
}