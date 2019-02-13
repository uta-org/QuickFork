using CommandLine;

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace QuickFork.Shell
{
    using Lib;
    using Lib.Model;

    internal class Runner
    {
        //private static FileVersionInfo VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

        //private static string WorkingPath
        //{
        //    get
        //    {
        //        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), VersionInfo.CompanyName, "SyncFolder");

        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        return path;
        //    }
        //}

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
            Parser.Default.ParseArguments<CLI>(args)
                   .WithParsed(cli =>
                   {
                       if (!string.IsNullOrEmpty(cli.RelinkPath))
                       {
                           try
                           {
                               ExecuteRelinker(cli.RelinkPath).GetAwaiter().GetResult();
                           }
                           catch (Exception ex)
                           {
                               Console.WriteLine(ex, Color.Red);
                           }
                           finally
                           {
                               Console.WriteLine("Press any key to exit...");
                               Console.Read();
                           }
                       }
                       else
                           MainExecution();
                   });
        }

        private static async Task ExecuteRelinker(string relinkerPath)
        {
            var slnFiles = Directory.GetFiles(relinkerPath, "*.sln", SearchOption.AllDirectories);

            if (slnFiles.Length == 0)
                throw new Exception("Provided path doesn't have any sln file.");

            string slnFile = slnFiles[0];

            CsProjLinking map = F.RetrieveDependencies(relinkerPath, true);

            foreach (var kv in map.Data)
            {
                // Working Path must be the same/has the same hierarchy as the project we specified on the relinkerPath (to avoid people contributing to a project break the solution file)
                string workingPath = Path.GetFullPath(Path.Combine(relinkerPath, kv.Key)),
                       workingFolder = !workingPath.IsDirectory() ? Path.GetDirectoryName(workingPath) : workingPath;

                if (!Directory.Exists(workingFolder))
                    Directory.CreateDirectory(workingFolder);

                // Test: This should fail due to a non-empty folder
                await GitHelper.CloneRepo(workingFolder, kv.Key, Path.GetFileNameWithoutExtension(kv.Key));
            }
        }

        private static void MainExecution()
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