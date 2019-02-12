using uzLib.Lite.Interoperability;

namespace QuickFork.Lib
{
    public static class StaticShell
    {
        public static GitShell MyShell { get; private set; }

        static StaticShell()
        {
            if (MyShell == null)
                MyShell = new GitShell();
        }
    }
}