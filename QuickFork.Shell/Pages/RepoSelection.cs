using EasyConsole;
using System;
using System.Collections.Generic;
using System.IO;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    using Common;

    internal sealed class RepoSelection : MenuPage
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

            list.AddNullableRange(RepoFunc.Get(item, (i, _item) => SelectRepo(i, _item)));

            list.Add(new Option("Create new local cloned repository", () => SelectRepo(-1, item)));
            list.Add(new Option("Remove repository from the list", () =>
            {
                CurrentProgram.AddPage(new RepoDeletion(CurrentProgram, CurrentItem));
                CurrentProgram.NavigateTo<RepoDeletion>();
            }));

            return list;
        }

        public static void SelectRepo(int index, ProjectItem pItem)
        {
            Console.WriteLine();

            RepoItem rItem;

            if (index == -1)
            {
                string gitUrl = "";
                bool isValid = false,
                     alreadyAdded = false;

                do
                {
                    Console.Write("Project Repo Url < .git extension >: ");
                    gitUrl = Console.ReadLine();

                    if (Forker.Repos.ContainsKey(gitUrl))
                    {
                        alreadyAdded = true;
                        break;
                    }

                    isValid = gitUrl.CheckURLValid();

                    if (!isValid)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid URL provided, please, type again.");
                    }

                    Console.WriteLine();
                }
                while (!isValid);

                if (!alreadyAdded)
                {
                    rItem = new RepoItem(gitUrl);
                    rItem.Update(pItem.SelectedPath);

                    Console.WriteLine("Repository has created succesfully!");
                }
                else
                {
                    rItem = Forker.Repos[pItem.SelectedPath][index];
                    Console.WriteLine($"This repository '{rItem.Name}' was already added!");
                }

                Console.WriteLine();
            }
            else
                rItem = Forker.Repos[pItem.SelectedPath][index];

            CurrentProgram.AddPage(new RepoOperation(CurrentProgram, rItem, pItem));
            CurrentProgram.NavigateTo<RepoOperation>();
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.Repos.IsNullOrEmpty(CurrentItem.SelectedPath);

            if (isNew)
            {
                Console.WriteLine("There isn't any available repo to select, please, create a new one.");
                SelectRepo(-1, CurrentItem);
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