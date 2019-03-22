using EasyConsole;

using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages
{
    using Common;
    using Lib;
    using Lib.Model;

    /// <summary>
    /// The Dependence Creator class (see <see cref="ProjectItem"/> CreateDependencies method)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal class DependenceCreator : MenuPage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="DependenceCreator"/> class from being created.
        /// </summary>
        private DependenceCreator()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependenceCreator"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public DependenceCreator(Program program)
            : base("Dependence Creator", program, () => GetOptions(program))
        {
        }

        /// <summary>
        /// Gets the options of the current page
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        // TODO: Implement here a list of projects (like ProjectList) + add/remove, and then, when you select any project, then you only get ProjectItem and call with this to RepoItem.CreateDependencies(pItem)
        private static GetOptionsDelegate GetOptions(Program program)
        {
            // TODO: Refactorize this
            var list = Forker.StoredProjects == null ? new List<Option>() : ProjectFunc.Get((index) =>
            {
                Forker.StoredProjects.ElementAt(index).CreateDependencies();

                Console.WriteLine();
                Console.WriteLine("Press any key to go back...");
                Console.Read();
                CurrentProgram.NavigateBack();
            }).ToList();

            list.AddRange(CommonFunc.CommonOptions<ProjectItem>(program, (newProject) =>
            {
                CurrentProgram.NavigateBack(true, PopAction.NoPop);
            }));

            return () => list;
        }
    }
}