using CommandLine;

namespace QuickFork.Shell
{
    public class CLI
    {
        [Option("relink", Required = false, HelpText = "Specify a relink path.")]
        public string RelinkPath { get; set; }
    }
}