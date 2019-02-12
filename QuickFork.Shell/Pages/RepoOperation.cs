using EasyConsole;
using System;
using System.Drawing;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    using Console = Colorful.Console;

    internal sealed class RepoOperation : MenuPage
    {
        private RepoOperation()
            : base("", null)
        {
        }

        public RepoOperation(Program program, RepoItem rItem, ProjectItem pItem)
            : base("Repository Operation", program,
                new Option($"Sync the '{rItem.Name}' repository to the '{pItem.Name}' project (clone + link)", () => Operate(null, rItem, pItem)),
                new Option($"Only clone the '{rItem.Name}' repository", () => Operate(true, rItem, pItem)),
                new Option($"Only link '{rItem.Name}' to the '{pItem.Name}' solution", () => Operate(false, rItem, pItem)),
                new Option("Add another repository", () => program.NavigateBack(-2)))
        {
        }

        private static void Operate(bool? doLinking, RepoItem rItem, ProjectItem pItem)
        {
            try
            {
                var csProjs = rItem?.Execute(pItem, pItem.Type, doLinking).GetAwaiter().GetResult();

                if ((!doLinking.HasValue || doLinking.HasValue && !doLinking.Value) &&
                    !Forker.IsAlreadyOnFile(RepoSelection.PackageFile, pItem.SelectedPath))
                    Forker.SerializeProject(RepoSelection.PackageFile, pItem, rItem, csProjs);
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