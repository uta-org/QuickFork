using EasyConsole;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    internal sealed class RepoSelection : MenuPage
    {
        private const int DashLength = 30;

        public static ProjectItem CurrentItem { get; private set; }

        public static string PackageFile => Path.Combine(CurrentItem.SelectedPath, "dependencies.json");

        private RepoSelection()
            : base("", null)
        {
        }

        public RepoSelection(Program program, ProjectItem item)
            : base("Repository Selection", program)
        {
            CurrentItem = item;
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.Repos.IsNullOrEmpty(CurrentItem.SelectedPath),
                 hasLinkedProjs;

            Console.WriteLine("Linked repositories to this project:", Color.White);
            Console.WriteLine(new string('-', DashLength), Color.Gray);

            List<RepoItem> repos = null;

            if (isNew)
            {
                Console.WriteLine("This project doesn't have any linked repository. Please, select one from the list below.", Color.LightBlue);
                hasLinkedProjs = false;
            }
            else
            {
                repos = Forker.Repos[CurrentItem.SelectedPath];
                int count = repos.Count;

                Console.WriteLine($"This project {(count == 1 ? "has" : "have")} {count} repository linked.", Color.LightBlue);
                Console.WriteLine();

                {
                    var repoMenus = new Menu();

                    hasLinkedProjs = repos.Any(r => Forker.RepoProjLinking.ContainsKey(r.Index) && !Forker.RepoProjLinking[r.Index].IsNullOrEmpty());
                    repoMenus.AddRange(repos.Select(r => new Option($"{r.Name} ({(hasLinkedProjs ? string.Join(", ", Forker.RepoProjLinking[r.Index]) : "This repo hasn't any CSProj linked.")})")));
                    repoMenus.DisplayOptions();
                }
            }

            Console.WriteLine(new string('-', DashLength), Color.Gray);

            {
                var repoMenus = new Menu(() => RepoList.GetOptions(CurrentProgram, (rItem) =>
                {
                    // Solved bug, this shouldn't be called on selection
                    // Forker.Add(CurrentItem, rItem);

                    Forker.UpdateMap(CurrentItem.SelectedPath, rItem.Index);

                    CurrentProgram.AddPage(new RepoOperation(CurrentProgram, rItem, CurrentItem));
                    CurrentProgram.NavigateTo<RepoOperation>();
                }, null, hasLinkedProjs ? new OptionAction("Remove linked csproj from solution", () =>
                {
                    int selectedRepo = -1;

                    var displayRepos = new Menu();

                    displayRepos.AddRange(repos.Select((r, i) => new Option(r.Name, () => selectedRepo = i)));
                    displayRepos.Display(false);

                    Console.WriteLine();

                    var selectedLinks = new List<int>();
                    var remLinkedProj = new Menu();

                    remLinkedProj.AddRange(Forker.RepoProjLinking[selectedRepo].Select((link, i) => new Option(link, () => selectedLinks.Add(i))));
                    remLinkedProj.Display(true);

                    Console.WriteLine();

                    foreach (int link in selectedLinks)
                        Forker.RemoveLinking(selectedRepo, link);

                    CurrentProgram.NavigateBack();
                }) : null));

                repoMenus.DisplayOptions();

                Console.WriteLine(new string('-', DashLength), Color.Gray);

                repoMenus.DisplayCaption(true);
            }
        }
    }
}