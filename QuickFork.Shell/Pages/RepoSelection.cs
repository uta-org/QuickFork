using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    public class RepoSelection : MenuPage
    {
        public static ProjectItem CurrentItem { get; private set; }

        public static string PackageFile => Path.Combine(CurrentItem.SelectedPath, "dependencies.json");

        private RepoSelection()
            : base("", null, null)
        {
        }

        public RepoSelection(Program program, ProjectItem item)
            : base("Repository Selection", program, GetOptions(item).ToArray())
        {
            CurrentItem = item;
        }

        public static List<Option> GetOptions(ProjectItem item)
        {
            List<Option> list = new List<Option>();

            if (Forker.Repos.Count > 0 && Forker.Repos.ContainsKey(item.SelectedPath))
                Forker.Repos[item.SelectedPath].AsEnumerable().ForEach((r, i) => list.Add(new Option(r.ToString(), () => SelectRepo(i, item))));

            list.Add(new Option("Create new local clone", () => SelectRepo(-1, item)));

            return list;
        }

        public static void SelectRepo(int index, ProjectItem pItem)
        {
            RepoItem rItem;

            if (index == -1)
            {
                Console.Write("Project Repo Url < .git extension >: ");
                string gitUrl = Console.ReadLine();
                Console.WriteLine();

                rItem = new RepoItem(gitUrl);

                Console.WriteLine("Repository has created succesfully!");
            }
            else
                rItem = Forker.Repos[pItem.SelectedPath][index];

            MainProgram.Instance.AddPage(new RepoOperation(MainProgram.Instance, rItem, pItem));
            MainProgram.Instance.NavigateTo<RepoOperation>();
        }

        public override void Display()
        {
            bool isNew = !Forker.Repos.HasValues(CurrentItem.SelectedPath);

            if (isNew)
            {
                Console.WriteLine("There isn't any available repo to select, please, create a new one.");
                Console.WriteLine();
                SelectRepo(-1, CurrentItem);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.Repos[CurrentItem.SelectedPath].Count} local repo available. Which do you wish to use?");
                Console.WriteLine();

                base.Display();
            }
        }
    }
}