using EasyConsole;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    internal sealed class RepoSelection : MenuPage
    {
        private const int DashLength = 30;

        public static ProjectItem CurrentItem { get; private set; }

        public static string PackageFile => CurrentItem.GetPackageFile();

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

                    hasLinkedProjs = repos.Any(r =>
                    {
                        int index = r.GetIndex();
                        return Forker.RepoProjLinking.ContainsKey(index) && !Forker.RepoProjLinking[index].IsNullOrEmpty();
                    });

                    repoMenus.AddRange(repos.Select(r =>
                    {
                        int index = r.GetIndex();
                        return new Option($"{r.Name} ({(hasLinkedProjs && Forker.RepoProjLinking.ContainsKey(index) ? string.Join(", ", Forker.RepoProjLinking[index]) : "This repo hasn't any CSProj linked.")})");
                    }));

                    repoMenus.DisplayOptions();
                }
            }

            Console.WriteLine(new string('-', DashLength), Color.Gray);

            {
                var repoMenus = new Menu(() => RepoList.GetOptions(CurrentProgram, SelectRepo, true, null,
                    hasLinkedProjs ? new OptionAction("Remove linked csproj from solution", () =>
                {
                    Console.WriteLine();
                    Console.WriteLineFormatted("{0} To remove the entire linked repository you must select '{1}' or go to '{2}' and remove from there.", Color.Yellow, Color.White, new[] { "Note:", "Delete the entire repository", "Repository List" });
                    Console.WriteLine();

                    int selectedRepo = -1;
                    bool goBack = false;

                    do
                    {
                        if (goBack)
                            goBack = false;

                        var displayRepos = new Menu();

                        displayRepos.AddRange(repos.Select((r, i) => new Option(r.Name, () => selectedRepo = i)));
                        displayRepos.Display(false);

                        Console.WriteLine();

                        var selectedLinks = new List<int>();
                        var remLinkedProj = new Menu();

                        bool allProjects = false;

                        if (Forker.RepoProjLinking[selectedRepo].Length > 1)
                            remLinkedProj.Add(new Option($"Delete the entire repository from '{CurrentItem.Name}'", () => allProjects = true));

                        remLinkedProj.AddRange(Forker.RepoProjLinking[selectedRepo].Select((link, i) => new Option(link, () => selectedLinks.Add(i))));
                        remLinkedProj.Add("Go back", () => goBack = true);

                        remLinkedProj.Display(true);

                        Console.WriteLine();

                        if (!goBack)
                        {
                            if (allProjects)
                                Forker.RemoveAllLinkings(CurrentItem, selectedRepo);
                            else
                                foreach (int link in selectedLinks)
                                    Forker.RemoveLinking(CurrentItem, selectedRepo, link);
                        }
                    }
                    while (goBack);

                    CurrentProgram.NavigateBack();
                }) : null));

                // Add all available repositories (available means that they aren't already added to this project)
                var notAddedRepos = Forker.StoredRepos.Where(r => !Forker.Repos[CurrentItem.SelectedPath].Contains(r));
                repoMenus.Insert(repoMenus.Count - 3, "Add all the available repositories", () => notAddedRepos.ForEach((_r) => SelectRepo(_r)));

                // Display available options...
                repoMenus.DisplayOptions();

                Console.WriteLine(new string('-', DashLength), Color.Gray);

                // Then, display caption to choose multiple options
                repoMenus.DisplayCaption(true);
            }
        }

        private static void SelectRepo(RepoItem rItem)
        {
            // This will update the RepoMap without adding new entries
            Forker.UpdateMap(CurrentItem.SelectedPath, rItem, false);

            CurrentProgram.AddPage(new RepoOperation(CurrentProgram, rItem, CurrentItem)); //.UpdateOptions(CurrentProgram, rItem, CurrentItem);
            CurrentProgram.NavigateTo<RepoOperation>();
        }
    }
}