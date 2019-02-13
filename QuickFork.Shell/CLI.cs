using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Shell
{
    public class CLI
    {
        [Option("relink", Required = false, HelpText = "Specify a relink path.")]
        public string RelinkPath { get; set; }
    }
}