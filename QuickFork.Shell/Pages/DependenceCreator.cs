using EasyConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Shell.Pages
{
    internal class DependenceCreator : MenuPage
    {
        private DependenceCreator()
            : base("", null)
        {
        }

        public DependenceCreator(Program program)
            : base("Dependence Creator", program, () => GetOptions(program))
        {
        }

        public override void Display(string caption = "Choose an option: ")
        {
            // Implement here a list of projects (like ProjectList) + add/remove, and then, when you select any project, then you only get ProjectItem and call with this to RepoItem.CreateDependencies(pItem)
            base.Display(caption);
        }

        private static GetOptionsDelegate GetOptions(Program program)
        {
            return () => null;
        }
    }
}