using EasyConsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace QuickFork.Shell.Pages.Repos
{
    using Lib;
    using Lib.Model;

    using Console = Colorful.Console;

    /// <summary>
    /// The RepoOperation class (the main operations available for Repositories are listed here)
    /// </summary>
    /// <seealso cref="EasyConsole.MenuPage" />
    internal sealed class RepoOperation : MenuPage
    {
        /// <summary>
        /// Gets or sets the name of the current repo.
        /// </summary>
        /// <value>
        /// The name of the current repo.
        /// </value>
        private string CurrentRepoName { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="RepoOperation"/> class from being created.
        /// </summary>
        private RepoOperation()
            : base("", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoOperation"/> class.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="rItem">The r item.</param>
        /// <param name="pItem">The p item.</param>
        public RepoOperation(Program program, RepoItem rItem, ProjectItem pItem)
            : base("Repository Operation", program, GetOptions(program, rItem, pItem).ToArray())
        {
            CurrentRepoName = rItem.Name;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="rItem">The r item.</param>
        /// <param name="pItem">The p item.</param>
        /// <returns></returns>
        private static IEnumerable<Option> GetOptions(Program program, RepoItem rItem, ProjectItem pItem)
        {
            yield return new Option($"Sync the '{rItem.Name}' repository to the '{pItem.Name}' project (clone + link)", () => Operate(null, rItem, pItem));
            yield return new Option($"Only clone the '{rItem.Name}' repository", () => Operate(true, rItem, pItem));
            yield return new Option($"Only link '{rItem.Name}' to the '{pItem.Name}' solution", () => Operate(false, rItem, pItem));
            yield return new Option("Add another repository", () => program.NavigateBack(-2));
        }

        /// <summary>
        /// Operates the specified do linking.
        /// </summary>
        /// <param name="doLinking">The do linking.</param>
        /// <param name="rItem">The r item.</param>
        /// <param name="pItem">The p item.</param>
        private static void Operate(bool? doLinking, RepoItem rItem, ProjectItem pItem)
        {
            try
            {
                var csProjs = rItem?.Execute(pItem, pItem.Type, doLinking);

                if ((!doLinking.HasValue || doLinking.HasValue && !doLinking.Value) &&
                    !Forker.IsAlreadyOnFile(RepoSelection.PackageFile, rItem.GitUrl))
                    Forker.SerializeProject(pItem, rItem, csProjs);
                else
                    Console.WriteLine($"The dependency you are trying to add to the '{pItem.Name}' project is already added!", Color.Yellow);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex, Color.Red);
            }
        }
    }
}