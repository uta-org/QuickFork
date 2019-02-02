using EasyConsole;
using System.Drawing;
using System.Collections.Generic;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;
    using Common;

    using Console = Colorful.Console;

    internal sealed class ProjectSelection : MenuPage
    {
        private ProjectSelection()
            : base("", null)
        {
        }

        public ProjectSelection(Program program)
            : base("Project Selection", program, () => GetOptions())
        {
        }

        private static GetOptionsDelegate GetOptions()
        {
            List<Option> list = new List<Option>();

            if (!Forker.StoredProjects.IsNullOrEmpty())
                Forker.StoredProjects.ForEach((pItem, i) => list.Add(new Option(pItem.Name, () =>
                {
                    var _pItem = ProjectFunc.Add(i);

                    CurrentProgram.AddPage(new ProjectOperation(CurrentProgram, _pItem));
                    CurrentProgram.NavigateTo<ProjectOperation>();
                })));

            list.AddRange(CommonFunc.CommonOptions<ProjectItem>(CurrentProgram, (pItem) =>
            {
                Console.WriteLine();

                ProjectFunc.Add();

                CurrentProgram.AddPage(new ProjectOperation(CurrentProgram, pItem));
                CurrentProgram.NavigateTo<ProjectOperation>();
            }));

            return () => list;
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.StoredProjects.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available project to select, please, create a new one.", Color.LightBlue);
                Console.WriteLine();
                ProjectFunc.Add();
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"This are the {Forker.StoredProjects.Count} local projects available. Which do you wish to use?");
                Console.WriteLine();

                base.Display(caption);
            }
        }
    }
}