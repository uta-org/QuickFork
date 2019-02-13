using System;
using System.Runtime.InteropServices;

namespace QuickFork.Shell
{
    using Lib;

    internal class Runner
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    Forker.SaveInstance();
                    return false;
            }
        }

        private static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            var program = new MainProgram().Run();

            if (!program.IsExiting)
            {
                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

        internal static void SafeExit()
        {
            // We call SaveInstance from here, because Env.Exit doesn't trigger handler.
            Forker.SaveInstance();
            Environment.Exit(0);
        }
    }
}