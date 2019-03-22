using EasyConsole;
using System.Collections.Generic;
using System.Drawing;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages.Projects
{
    using Common;
    using Lib;
    using Lib.Model;
    using Console = Colorful.Console;

    /// <summary>
    /// The ProjectSelection class (the Project are listed here to select them)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal sealed class ProjectSelection : MenuPage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ProjectSelection"/> class from being created.
        /// </summary>
        private ProjectSelection()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelection"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public ProjectSelection(Program program)
            : base("Project Selection", program, () => GetOptions())
        {
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Displays the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        public override void Display(string caption = "Choose an option: ")
        {
            bool isNew = Forker.StoredProjects.Count == 0;

            if (isNew)
            {
                Console.WriteLine("There isn't any available project to select, please, create a new one.", Color.LightBlue);
                Console.WriteLine();

                ProjectItem pItem = ProjectFunc.Add();

                CurrentProgram.AddPage(new ProjectOperation(CurrentProgram, pItem));
                CurrentProgram.NavigateTo<ProjectOperation>();
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