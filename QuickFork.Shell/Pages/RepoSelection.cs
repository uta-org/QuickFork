using EasyConsole;
using System.Collections.Generic;
using System.IO;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    using Common;
    using System.Drawing;

    internal sealed class RepoSelection : MenuPage
    {
        public static ProjectItem CurrentItem { get; private set; }

        public static string PackageFile => Path.Combine(CurrentItem.SelectedPath, "dependencies.json");

        private RepoSelection()
            : base("", null)
        {
        }

        public RepoSelection(Program program, ProjectItem item)
            : base("Repository Selection", program, GetOptions(item).ToArray())
        {
            CurrentItem = item;
        }

        public static List<Option> GetOptions(ProjectItem pItem)
        {
            List<Option> list = new List<Option>();

            list.AddNullableRange(RepoFunc.Get(pItem, (i, _item) => RepoFunc.RepoAdd(i, _item)));

            list.AddRange(RepoFunc.CommonRepoOptions(CurrentProgram, (rItem) =>
            {
                Console.WriteLine();

                RepoFunc.RepoAdd(-1, pItem);

                CurrentProgram.AddPage(new RepoOperation(CurrentProgram, rItem, pItem));
                CurrentProgram.NavigateTo<RepoOperation>();

                Console.WriteLine();
            }));

            return list;
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.Repos.IsNullOrEmpty(CurrentItem.SelectedPath);

            if (isNew)
            {
                Console.WriteLine("There isn't any available repo to select, please, create a new one.", Color.LightBlue);
                RepoFunc.RepoAdd(-1, CurrentItem);
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