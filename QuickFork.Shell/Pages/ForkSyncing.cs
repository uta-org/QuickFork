using EasyConsole;
using QuickFork.Lib;
using System;
using System.Collections.Generic;
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
            Forker.LoadSettings();
        }

        public override void Display()
        {
            bool isNew = Forker.RepoCollection.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available prject to select, please, create a new one:");
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

                Console.Write("Project Folder Path: ");
                string folderPath = Console.ReadLine();

                repoItem = Forker.Fork(gitUrl, folderPath);

                Console.WriteLine("Project has created succesfully!");
            }
            else
                repoItem = Forker.RepoCollection[index];

            DisplayNewOptions();

            Console.Write("Write the path to your project: ");
            string projectPath = Console.ReadLine();

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

        private static void DisplayNewOptions()
        {
            var newForkMenu = new Menu();
            NewForkOptions().ForEach(o => newForkMenu.Add(o));

            newForkMenu.Display();
        }

        public static IEnumerable<Option> NewForkOptions()
        {
            yield return new Option("Search .csproj and add to solution specified", () => SetType(OperationType.AddProjToSLN));
            yield return new Option("Create a symlink", () => SetType(OperationType.CreateSymlink));
        }
    }
}