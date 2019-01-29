using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell.Pages
{
    using Lib;
    using Lib.Model;

    internal class RepoOperation : MenuPage
    {
        private static bool? DoLinking { get; set; }

        private RepoOperation()
            : base("", null, null)
        {
        }

        public RepoOperation(Program program, RepoItem rItem, ProjectItem pItem)
            : base("Repository Operations", program,
                new Option("Sync the entire repository to a project", () => Operate(null, rItem, pItem)),
                new Option("Only clone the repository", () => Operate(true, rItem, pItem)),
                new Option("Link an existing repository to an existing solution", () => Operate(false, rItem, pItem)))
        {
        }

        private static void Operate(bool? value, RepoItem rItem, ProjectItem pItem)
        {
            DoLinking = value;

            try
            {
                rItem?.Execute(pItem.SelectedPath, pItem.Type, DoLinking);
                File.WriteAllText(RepoSelection.PackageFile, JsonConvert.SerializeObject(new Forker(pItem.SelectedPath), Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The following occured: {ex.Message}");
            }
        }
    }
}