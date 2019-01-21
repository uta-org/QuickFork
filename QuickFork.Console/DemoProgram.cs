using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Console
{
    using Pages;

    internal class DemoProgram : Program
    {
        public DemoProgram()
            : base("EasyConsole Demo", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));
            AddPage(new ForkSyncing(this));

            SetPage<MainPage>();
        }
    }
}