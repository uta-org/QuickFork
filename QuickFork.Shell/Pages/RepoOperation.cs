using EasyConsole;
using System;
using System.IO;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    internal class RepoOperation : MenuPage
    {
        private RepoOperation()
            : base("", null, null)
        {
        }

        public RepoOperation(Program program, RepoItem rItem, ProjectItem pItem)
            : base("Repository Operation", program,
                new Option($"Sync the '{rItem.Name}' repository to the '{pItem.Name}' project (clone + link)", () => Operate(null, rItem, pItem)),
                new Option($"Only clone the '{rItem.Name}' repository", () => Operate(true, rItem, pItem)),
                new Option($"Only link '{rItem.Name}' to the '{pItem.Name}' solution", () => Operate(false, rItem, pItem)))
        {
        }

        private static void Operate(bool? doLinking, RepoItem rItem, ProjectItem pItem)
        {
            try
            {
                Forker.Add(pItem.SelectedPath, rItem);

                rItem?.Execute(pItem.SelectedPath, pItem.Type, doLinking);

                if (!doLinking.HasValue || doLinking.HasValue && !doLinking.Value)
                    File.WriteAllText(RepoSelection.PackageFile, Forker.SerializeProject(pItem.SelectedPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The following {ex.GetType().Name} occured: {ex.Message}");
            }
        }
    }
}