using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Console
{
    using Console = System.Console;
    using Pages;

    internal class MainPage : MenuPage
    {
        private MainPage()
            : base("", null, null)
        {
        }

        public MainPage(Program program)
            : base("Main Page", program,
          new Option("Fork Syncing", () => program.NavigateTo<ForkSyncing>()),
          new Option("Exit", () => Environment.Exit(0)))
        {
        }

        /*public override void Display()
        {
            base.Display();
        }*/

        /*private static void Main(string[] args)
        {
            var menu = new EasyConsole.Menu()
              .Add("Sync a fork", () => SyncFork())
              .Add("Exit", () => Environment.Exit(0));

            menu.Display();
        }

        private static void SyncFork()
        {
        }*/
    }
}