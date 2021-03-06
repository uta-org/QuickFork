﻿using CommandLine;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using uzLib.Lite.Extensions;
using Console = Colorful.Console;

namespace QuickFork.Shell
{
    using Lib;
    using Lib.Model;

    /// <summary>
    /// The Runner class
    /// </summary>
    internal class Runner
    {
        /// <summary>
        /// Sets the console control handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="add">if set to <c>true</c> [add].</param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        /// <summary>
        /// Event Handler used by the Hook
        /// </summary>
        /// <param name="sig">The sig.</param>
        /// <returns></returns>
        private delegate bool EventHandler(CtrlType sig);

        /// <summary>
        /// The handler
        /// </summary>
        private static EventHandler _handler;

        /// <summary>
        /// CtrlType enum
        /// </summary>
        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        /// <summary>
        /// Handlers the specified sig.
        /// </summary>
        /// <param name="sig">The sig.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CLI>(args)
                   .WithParsed(cli =>
                   {
                       // Relinker entry
                       if (!string.IsNullOrEmpty(cli.RelinkPath))
                       {
                           try
                           {
                               Console.WriteAscii("RELINKING", Color.White);
                               Console.WriteWithGradient(Enumerable.Repeat('-', 30), Color.Red, Color.Green, 10);
                               Console.WriteLine();

                               ExecuteRelinker(cli.RelinkPath);
                           }
                           catch (Exception ex)
                           {
                               Console.WriteLine(ex, Color.Red);
                           }
                           finally
                           {
#if DEBUG
                               Console.WriteLine("Press any key to exit...");
                               Console.Read();
#endif
                           }
                       }
                       else
                           MainExecution();
                   })
                   .WithNotParsed((errors) =>
                   {
                       foreach (var e in errors)
                           Console.WriteLine(e);

                       Console.Read();
                   });
        }

        // This must work with the "$(SolutionDir)" from Tools > External Tools...
        /// <summary>
        /// Executes the relinker.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        /// <exception cref="ArgumentException">
        /// Specified path was not a folder. - rootFolder
        /// or
        /// Specefied folder doesn't contain any 'dependencies.json'.
        /// </exception>
        /// <exception cref="Exception">There is an inconsistence on the 'dependencies.json' file. The same repository can't contain different top-level folders.</exception>
        private static void ExecuteRelinker(string rootFolder)
        {
            if (!rootFolder.IsDirectory())
                throw new ArgumentException("Specified path was not a folder.", "rootFolder");

            if (!F.HasDependencies(rootFolder))
                throw new ArgumentException("Specefied folder doesn't contain any 'dependencies.json'.");

            CsProjLinking map = F.RetrieveDependencies(rootFolder, true);

            foreach (var kv in map.Data)
            {
                // Working Path must be the same/has the same hierarchy as the project we specified on the relinkerPath (to avoid people contributing to a project break the solution file)

                // This a little bit more difficult than expected. Why? On the kv.Values we have the csproj files. But we don't know exactly where are the root folder of this projects (where solution is located).
                // So, we will asume that the last folder of this relative folder is where we need to clone everything.

                string workingFolder = IOHelper.GetTopLevelDir(kv.Value[0]);

                if (kv.Value.Any(pth => IOHelper.GetTopLevelDir(pth) != workingFolder))
                    throw new Exception("There is an inconsistence on the 'dependencies.json' file. The same repository can't contain different top-level folders.");

                string workingPath = Path.GetFullPath(Path.Combine(rootFolder, workingFolder));

                if (!Directory.Exists(workingPath))
                    Directory.CreateDirectory(workingPath);

                string repoName = Path.GetFileNameWithoutExtension(kv.Key);
                GitHelper.CloneRepo(workingPath, kv.Key, repoName);

                Console.WriteLine($"Succesfully cloned '{repoName}' into '{workingPath}'!", Color.DarkGreen);
            }
        }

        /// <summary>
        /// The main the execution.
        /// </summary>
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

        /// <summary>
        /// Make a safe exit.
        /// </summary>
        internal static void SafeExit()
        {
            // We call SaveInstance from here, because Env.Exit doesn't trigger handler.
            Forker.SaveInstance();
            Environment.Exit(0);
        }
    }
}