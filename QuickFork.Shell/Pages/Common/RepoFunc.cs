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
            RepoItem rItem = null;
            bool alreadyAdded = false, showWarning = false;

            if (index == -1)
            {
                string gitUrl = "";
                bool isValid = false;

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
                    showWarning = true;
            }
            else
                alreadyAdded = true;

            if (alreadyAdded)
            {
                rItem = Forker.Repos[pItem.SelectedPath][index];

                if (showWarning)
                    Console.WriteLine($"This repository '{rItem.Name}' was already added!", Color.Yellow);
            }

            return rItem;
        }
    }
}