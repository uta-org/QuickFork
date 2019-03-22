using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace QuickFork.Shell.Pages.Projects
{
    using Lib;
    using Lib.Model;
    using Repos;

    /// <summary>
    /// The ProjectOperation class (the main operations available of the Projects are listed here)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal sealed class ProjectOperation : MenuPage
    {
        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        /// <value>
        /// The current item.
        /// </value>
        private static ProjectItem CurrentItem { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ProjectOperation"/> class from being created.
        /// </summary>
        private ProjectOperation()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectOperation"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="item">The item.</param>
        public ProjectOperation(Program program, ProjectItem item)
            : base("Project Operation", program, GetOptions().ToArray())
        {
            CurrentItem = item;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Option> GetOptions()
        {
            yield return new Option("Search .csproj and add to solution specified", () => Set(OperationType.AddProjToSLN));
            yield return new Option("Create a symlink", () => Set(OperationType.CreateSymlink));
        }

        /// <summary>
        /// Sets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        private static void Set(OperationType type)
        {
            CurrentItem.Type = type;

            CurrentProgram.AddPage(new RepoSelection(CurrentProgram, CurrentItem));
            CurrentProgram.NavigateTo<RepoSelection>();
        }
    }
}