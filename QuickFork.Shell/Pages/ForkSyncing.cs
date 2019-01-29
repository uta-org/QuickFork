using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    internal class ForkSyncing : MenuPage
    {
        public static bool? DoLinking { get; set; }

        private ProjectItem Item { get; }

        private ForkSyncing()
            : base("", null, null)
        {
        }

        public ForkSyncing(Program program, ProjectItem item)
            : base("Fork Syncing", program, GetOptions(item).ToArray())
        {
            Item = item;
        }

        public static List<Option> GetOptions(ProjectItem item)
        {
            List<Option> list = new List<Option>();

            if (Forker.Repos.Count > 0 && Forker.Repos.ContainsKey(item.SelectedPath))
                Forker.Repos[item.SelectedPath].AsEnumerable().ForEach((r, i) => list.Add(new Option(r.ToString(), () => SelectRepo(i, item))));

            list.Add(new Option("Create new local clone", () => SelectRepo(-1, item)));

            return list;
        }

        public static void SelectRepo(int index, ProjectItem item)
        {
            RepoItem repoItem;

            if (index == -1)
            {
                Console.Write("Project Repo Url < .git extension >: ");
                string gitUrl = Console.ReadLine();
                Console.WriteLine();

                repoItem = new RepoItem(gitUrl);

                Console.WriteLine("Repo has created succesfully!");
            }
            else
                repoItem = Forker.Repos[item.SelectedPath][index];

            try
            {
                repoItem?.Execute(item.SelectedPath, item.Type, DoLinking);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The following occured: {ex.Message}");
            }
        }

        public override void Display()
        {
            bool isNew = !Forker.Repos.HasValues(Item.SelectedPath);

            if (isNew)
            {
                Console.WriteLine("There isn't any available repo to select, please, create a new one.");
                Console.WriteLine();
                SelectRepo(-1, Item);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.Repos[Item.SelectedPath].Count} local repo available. Which do you wish to use?");
                Console.WriteLine();

                base.Display();
            }
        }
    }
}