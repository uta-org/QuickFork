using EasyConsole;
using QuickFork.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    internal class ForkSyncing : MenuPage
    {
        private static OperationType Type;

        private ForkSyncing()
            : base("", null, null)
        {
        }

        public ForkSyncing(Program program)
            : base("Fork Syncing", program, GetOptions().ToArray())
        {
            // Instance = this;
            Forker.LoadSettings();
        }

        public override void Display()
        {
            bool isNew = Forker.RepoCollection.Count == 0;

            if (isNew)
            {
                base.Display();
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.RepoCollection.Count} local projects available. Which do you wish to use?");
                Console.WriteLine();

                base.Display();
            }

            if (isNew)
                Console.WriteLine();
        }

        public static void SelectProject(int index)
        {
            if (index == -1)
            {
                Console.Write("Project Repo Url: ");
                string gitUrl = Console.ReadLine();

                Console.Write("Project Folder Path: ");
                string folderPath = Console.ReadLine();

                var repoItem = Forker.Fork(gitUrl, folderPath);

                Console.WriteLine("Project has created succesfully!");
                DisplayNewOptions();

                return;
            }

            DisplayNewOptions();
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
            else
            {
                list.AddRange(NewForkOptions());
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