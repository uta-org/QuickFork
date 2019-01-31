using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Shell.Pages
{
    internal class ProjectList : MenuPage
    {
        private ProjectList()
            : base("", null, null)
        {
        }

        public ProjectList(Program program)
            : base("Project List", program, GetOptions().ToArray())
        {
        }
    }
}