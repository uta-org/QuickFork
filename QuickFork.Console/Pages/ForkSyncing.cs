using EasyConsole;
using QuickFork.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Console.Pages
{
    using Console = System.Console;

    internal class ForkSyncing : MenuPage
    {
        public static ForkSyncing Instance { get; private set; }

        private OperationType type;

        private ForkSyncing()
            : base("", null, null)
        {
        }

        public ForkSyncing(Program program)
            : base("Fork Syncing", program,
                  new Option("Search .csproj and add to solution specified", () => SetType(OperationType.AddProjToSLN)),
                  new Option("Create a symlink", () => SetType(OperationType.CreateSymlink)))
        {
            Instance = this;
        }

        public override void Display()
        {
            base.Display();

            Console.WriteLine();

            // Display already saved RepoItem if not force to add one and do the process
            // Forker.Fork();

            Console.WriteLine();
        }

        public static void SetType(OperationType type)
        {
            Instance.type = type;
        }
    }
}