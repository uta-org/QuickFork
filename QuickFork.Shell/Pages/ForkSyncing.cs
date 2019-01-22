using EasyConsole;
using QuickFork.Lib;
using QuickFork.Lib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    internal class ForkSyncing : MenuPage
    {
        public static ForkSyncing Instance { get; private set; }
        public bool? DoLinking { get; set; }

        private static OperationType Type;

        private ForkSyncing()
            : base("", null, null)
        {
        }

        public ForkSyncing(Program program)
            : base("Fork Syncing", program, GetOptions().ToArray())
        {
            Instance = this;
        }

        public static List<Option> GetOptions()
        {
            List<Option> list = new List<Option>();

            if (Forker.RepoCollection.Count > 0)
            {
                Forker.RepoCollection.AsEnumerable().ForEach((r, i) => list.Add(new Option(r.ToString(), () => SelectProject(i))));
                list.Add(new Option("Create new local clone", () => SelectProject(-1)));
            }

            return list;
        }

        public override void Display()
        {
            bool isNew = Forker.RepoCollection.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available project to select, please, create a new one.");
                Console.WriteLine();
                SelectProject(-1);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.RepoCollection.Count} local projects available. Which do you wish to use?");
                Console.WriteLine();

                base.Display();
            }
        }

        public static void SelectProject(int index)
        {
            RepoItem repoItem;

            if (index == -1)
            {
                Console.Write("Project Repo Url: ");
                string gitUrl = Console.ReadLine();
                Console.WriteLine();

                repoItem = Forker.Fork(gitUrl);

                Console.WriteLine("Project has created succesfully!");
            }
            else
                repoItem = Forker.RepoCollection[index];

            DisplayNewOptions();

            string projectPath = "";
            int selectedProject = -1;
            bool newProject = true;

            if (Settings.Default.StoredFolders.Count > 0)
            {
                Console.WriteLine();

                var projectsMenu = new Menu();

                Settings.Default.StoredFolders.Cast<string>().ForEach((path, i) => projectsMenu.Add(Path.GetDirectoryName(path), () => { selectedProject = i; newProject = false; }));
                projectsMenu.Add("Add new project", () => selectedProject = -1);

                projectsMenu.Display();

                if (!newProject)
                    projectPath = Settings.Default.StoredFolders[selectedProject];
            }

            if (newProject)
                projectPath = ConsoleHelper.GetValidPath("Write the path to your project: ");

            try
            {
                repoItem.Execute(projectPath, Type, Instance.DoLinking);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The following occured: {ex.Message}");
            }
        }

        public static void SetType(OperationType type)
        {
            Type = type;
        }

        private static void DisplayNewOptions()
        {
            Console.WriteLine();

            var newForkMenu = new Menu();
            NewForkOptions().ForEach(o => newForkMenu.Add(o));

            newForkMenu.Display();

            Console.WriteLine();
        }

        public static IEnumerable<Option> NewForkOptions()
        {
            yield return new Option("Search .csproj and add to solution specified", () => SetType(OperationType.AddProjToSLN));
            yield return new Option("Create a symlink", () => SetType(OperationType.CreateSymlink));
        }
    }
}