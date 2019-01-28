using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using CreateToolkitShortcut;


namespace CreateShortcut
{
    /// <summary>
    /// Taken From: https://www.techpowerup.com/forums/threads/creating-shortcuts-with-batch-file.204740/
    /// Created By: FordGT90Concept
    /// Customized and Improved By: Cozy1
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            ConsoleColor normalColor = ConsoleColor.Green;
            ConsoleColor warningColor = ConsoleColor.White;
            ConsoleColor errorColor = ConsoleColor.Yellow;

            try
            {
                Console.SetWindowSize(80, 30);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = normalColor;
            }
            catch {/* DO NOTHING */}

            Console.WriteLine("Creating Shortcut ...");
            Console.WriteLine();

            // force the use of static arguments
            if (args.Length == 0)
            {
                // give the progie some args to work with
                Console.WriteLine("Using Internal Static Arguments for RocksmithToolkitGUI ...");
                Console.WriteLine();
                args = new string[]
                    {
                        "RocksmithToolkitGUI.lnk",
                        "./RocksmithToolkit/RocksmithToolkitGUI.exe",
                        "./RocksmithToolkit/songcreator.ico",
                        "0"
                    };
            }

            if (args.Length < 2) args = new string[] { "?" };
            if (args[0].Contains("?") || args[0].ToLower().Contains("help"))
            {
                Console.WriteLine(@" - CLI: CreateShortcut.exe");
                Console.WriteLine(@" - Version: " + ProjectVersion());
                Console.WriteLine();
                Console.WriteLine(@" - Purpose: Creates a shortcut for a file based on its relative path");
                Console.WriteLine();
                Console.WriteLine(@" - Command Line Usage: CreateShortcut [arg1] [arg2] [arg3] [arg4]");
                Console.WriteLine();
                Console.WriteLine(@" - Where: [arg1] shortcut relative path, e.g. 'myshortcut.lnk'");
                Console.WriteLine(@"          [arg2] application relative path, e.g. './programs/myshortcut.exe'");
                Console.WriteLine(@"          [arg3] icon relative path, e.g. './programs/myshortcut.ico'");
                Console.WriteLine(@"          [arg4] icon index, e.g. '0'");
                Console.WriteLine();
                Console.WriteLine(@" - Note: The paths are relative to location of CreateShortcut.exe");
                Console.WriteLine(@"         CreateToolkitShortcut.exe self destructs after sucessful completion");
                Console.WriteLine();
                Console.WriteLine(@"Press any key to continue ...");
                Console.Read();
                return 0;
            }

            FileInfo save = null, app = null, icon = null;
            int iconindex = 0;
            int result = 0; // completed sucessfully

            switch (args.Length)
            {
                case 2:
                    save = new FileInfo(args[0]);
                    app = new FileInfo(args[1]);
                    if (!app.Exists)
                    {
                        Console.ForegroundColor = warningColor;
                        Console.WriteLine("<WARNING> [arg2] application to launch not found.");
                        Console.WriteLine();
                        Console.ForegroundColor = normalColor;
                        result = 2;
                    }
                    break;
                case 3:
                    icon = new FileInfo(args[2]);

                    if (!icon.Exists)
                    {
                        try
                        {
                            iconindex = Convert.ToInt32(args[2]);
                        }
                        catch
                        {
                            Console.ForegroundColor = warningColor;
                            Console.WriteLine("<WARNING> [arg3] is not a valid index or file,");
                            Console.WriteLine("          assuming index 0 in application file.");
                            Console.WriteLine();
                            Console.ForegroundColor = normalColor;
                            result = 3;
                        }
                    }
                    goto case 2;

                case 4:
                    try
                    {
                        iconindex = Convert.ToInt32(args[3]);
                    }
                    catch
                    {
                        Console.ForegroundColor = warningColor;
                        Console.WriteLine("<WARNING> [arg4] is not a valid index,");
                        Console.WriteLine("          assuming index 0.");
                        Console.WriteLine();
                        Console.ForegroundColor = normalColor;
                        result = 4;
                    }
                    goto case 3;
            }

            if (save != null && app != null && (result == 0 || result > 2))
            {
                ShellShortcut shortcut = new ShellShortcut(save.FullName);
                shortcut.WorkingDirectory = app.DirectoryName;
                shortcut.Path = app.FullName;

                if (icon != null && icon.Exists)
                    shortcut.IconPath = icon.FullName;
                else
                    shortcut.IconPath = app.FullName;

                shortcut.IconIndex = iconindex;
                shortcut.Save();

                Console.WriteLine();
                Console.WriteLine(@"Sucessfully Creating Shortcut ...");
            }

            Console.WriteLine();
            Console.WriteLine(@" - [arg1] shortcut relative path: " + args[0]);
            Console.WriteLine(@" - [arg2] application relative path: " + args[1]);
            Console.WriteLine(@" - [arg3] icon relative path: " + args[2]);
            Console.WriteLine(@" - [arg4] icon index: " + args[3]);
            Console.WriteLine();

            // pause
            if (result != 0)
            {
                if (args[1] == "./RocksmithToolkit/RocksmithToolkitGUI.exe")
                {
                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("<ERROR> Create Toolkit Shortcut Failed ...");
                    Console.WriteLine();
                    Console.WriteLine(@" - Put the 'CreateToolkitShortcut.exe' file into the root folder");
                    Console.WriteLine(@"   of 'RocksmithToolkit' subfolder, and run it from there");
                    Console.WriteLine();
                    Console.ForegroundColor = normalColor;
                }

                Console.WriteLine(@"Press any key to continue ...");
                Console.Read();
                return result;
            }

            SelfDestruct.DoIt();
            return result;
        }

        private static string ProjectVersion()
        {
            return String.Format("{0}.{1}.{2}",
                                 Assembly.GetExecutingAssembly().GetName().Version.Major,
                                 Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                 Assembly.GetExecutingAssembly().GetName().Version.Build);
        }


    }
}
