using EasyConsole;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
            : base("Project Selection", program, GetOptions().ToArray())
        {
        }

        private static List<Option> GetOptions()
        {
            List<Option> list = new List<Option>();

            if (Forker.StoredProjects.Count > 0)
                Forker.StoredProjects.Cast<string>().ForEach((path, i) => list.Add(new Option(Path.GetFileName(path), () => ProjectFunc.Add(i))));

            list.AddRange(CommonFunc.CommonOptions<ProjectItem>(CurrentProgram, (pItem) =>
            {
                Console.WriteLine();

                ProjectFunc.Add();

                CurrentProgram.AddPage(new ProjectOperation(CurrentProgram, pItem));
                CurrentProgram.NavigateTo<ProjectOperation>();

                Console.WriteLine();
            }));

            return list;
        }

        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.StoredProjects.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available project to select, please, create a new one.", Color.LightBlue);
                Console.WriteLine();
                ProjectFunc.Add(-1);
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