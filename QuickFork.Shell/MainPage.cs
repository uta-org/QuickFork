using EasyConsole;
using uzLib.Lite.Extensions;

namespace QuickFork.Shell
{
    using Lib.Properties;

    internal class MainPage : MenuPage
    {
        private static Settings LibSettings => Settings.Default;

        private MainPage()
            : base("", null, null)
        {
        }

        public MainPage(Program program, params Option[] options)
            : base("Main Page", program, options)
        {
        }

        public override void Display()
        {
            if (string.IsNullOrEmpty(LibSettings.SyncFolder))
            {
                string syncPath = ConsoleHelper.GetValidPath("First of all, please, set the base folder where new repositories will be cloned: ");

                LibSettings.SyncFolder = syncPath;
                LibSettings.Save();
            }

            base.Display();
        }
    }
}