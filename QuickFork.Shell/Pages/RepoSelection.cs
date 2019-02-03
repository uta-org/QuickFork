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

        private static IEnumerable<Option> GetOptions(ProjectItem pItem)
        {
            return RepoFunc.Get(pItem, null);
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.Repos.IsNullOrEmpty(CurrentItem.SelectedPath);

            Console.WriteLine("Linked repositories to this project:", Color.White);
            Console.WriteLine(new string('-', DashLength), Color.Gray);

            if (isNew)
                Console.WriteLine("This project doesn't have any linked repository. Please, select one from the list below.", Color.LightBlue);
            else
            {
                int count = Forker.Repos[CurrentItem.SelectedPath].Count;

                Console.WriteLine($"This project {(count == 1 ? "has" : "have")} {count} repository linked.", Color.LightBlue);
                Console.WriteLine();

                var repoMenus = new Menu();

                repoMenus.AddRange(Forker.Repos[CurrentItem.SelectedPath].Select(r => new Option(r.Name, null)));
                repoMenus.DisplayOptions();
            }

            Console.WriteLine(new string('-', DashLength), Color.Gray);

            Console.WriteLine("All available repository:", Color.White);

            {
                Console.WriteLine(new string('-', DashLength), Color.Gray);

                var repoMenus = new Menu(() => RepoList.GetOptions(CurrentProgram, (rItem) =>
                {
                    Forker.Add(CurrentItem, rItem);

                    CurrentProgram.AddPage(new RepoOperation(CurrentProgram, rItem, CurrentItem));
                    CurrentProgram.NavigateTo<RepoOperation>();
                }));
                repoMenus.DisplayOptions();

                Console.WriteLine(new string('-', DashLength), Color.Gray);

                repoMenus.DisplayCaption(true);
            }
        }
    }
}