using System;

namespace QuickFork.Shell
{
    internal class Runner
    {
        private static void Main(string[] args)
        {
            new DemoProgram().Run();

            Console.Read();
        }
    }
}