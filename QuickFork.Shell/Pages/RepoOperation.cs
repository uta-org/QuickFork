using EasyConsole;
using System;
using System.IO;
using Newtonsoft.Json;

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
                rItem?.Execute(pItem.SelectedPath, pItem.Type, doLinking);
                File.WriteAllText(RepoSelection.PackageFile, JsonConvert.SerializeObject(new Forker(pItem.SelectedPath), Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The following occured: {ex.Message}");
            }
        }
    }
}