using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Shell.Pages.Common
{
    using Lib;
    using Lib.Model;

    internal static class RepoFunc
    {
        public static IEnumerable<Option> Get(ProjectItem item, Action<int, ProjectItem> action)
        {
            if (Forker.Repos.Count > 0 && Forker.Repos.ContainsKey(item.SelectedPath))
                return Forker.Repos[item.SelectedPath].AsEnumerable().Select((r, i) => new Option(r.ToString(), () => action?.Invoke(i, item)));
            else
                return null;
        }

        /*public static List<Option> GetRepoOptions(Program program, Action<int, RepoItem> action)
        {
            List<Option> list = new List<Option>();

            // list.AddNullableRange(RepoFunc.Get(item, (i, _item) => SelectRepo(i, _item)));
            list.AddNullableRange(Forker.StoredRepos.Select((r, i) => new Option(r.Name, () => action?.Invoke(i, Forker.StoredRepos.ElementAt(i)))));

            list.Add(new Option("Create new local cloned repository", () => SelectRepo(-1, item)));
            list.Add(new Option("Remove repository from the list", () =>
            {
                program.AddPage(new RepoDeletion(CurrentProgram, CurrentItem));
                program.NavigateTo<RepoDeletion>();
            }));

            return list;
        }*/

        public static IEnumerable<Option> CommonRepoOptions(Program program, Action<RepoItem> addAction)
        {
            yield return new Option("Create new local cloned repository", () => addAction?.Invoke(RepoAdd()));
            yield return new Option("Remove repository from the list", () =>
            {
                program.AddPage(new RepoDeletion(program));
                program.NavigateTo<RepoDeletion>();
            });
        }

        public static IEnumerable<Option> GetRepoList(Action<int> selectedRepo)
        {
            return Forker.StoredRepos?.Select((r, i) => new Option(r.Name, () => selectedRepo?.Invoke(i)));
        }

        public static RepoItem RepoAdd()
        {
            return RepoAdd(-1, null);
        }

        public static RepoItem RepoAdd(int index = -1, ProjectItem pItem = null)
        {
            RepoItem rItem;

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
                    Console.WriteLine("Invalid URL provided, please, type again.", Color.Red);
                }

                Console.WriteLine();
            }
            while (!isValid);

            if (!alreadyAdded)
            {
                if (pItem == null)
                    rItem = RepoItem.Update(gitUrl);
                else
                    rItem = RepoItem.Update(pItem.SelectedPath, gitUrl);

                Console.WriteLine("Repository has created succesfully!", Color.Green);
            }
            else
            {
                if (index > -1)
                {
                    rItem = Forker.Repos[pItem.SelectedPath][index];
                    Console.WriteLine($"This repository '{rItem.Name}' was already added!", Color.Yellow);
                }

                rItem = null;
            }

            Console.WriteLine();

            return rItem;
        }
    }
}