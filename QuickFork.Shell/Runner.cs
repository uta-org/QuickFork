using System;

namespace QuickFork.Shell
{
    internal class Runner
    {
        private static void Main(string[] args)
        {
            var program = new DemoProgram().Run();

            if (!program.IsExiting)
            {
                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }
    }
}