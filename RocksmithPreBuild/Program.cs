using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Globalization;


namespace RocksmithPreBuild
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // common variables are here
            var assemblyVersion = "0.0.0.0";
            var assemblyInformationVersion = "00000000"; // aka gitSubVersion
            var assemblyConfiguration = ""; // BUILD, BETA, RELEASE, empty string, or DateTime string
            var appExe = Assembly.GetExecutingAssembly().Location;
            var appPath = Path.GetDirectoryName(appExe);
            var parentPath = Path.GetDirectoryName(appPath);
            var gitHeadPath = Path.Combine(appPath, ".git", "HEAD");
            var patchAssemblyVersionPath = Path.Combine(appPath, "PatchAssemblyVersion.ps1");
            var toolkitVersionPath = Path.Combine(appPath, "RocksmithToolkitLib", "ToolkitVersion.cs");
            var versionInfoPath = Path.Combine(appPath, "VersionInfo.txt");

            // set CLI appearance
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            // commented out ... causes error when exe is run from the toolkit prebuild event
            // Console.SetWindowPosition(0, 0);
            // Console.SetWindowSize(90, 35);

            // feed the CLI some data when working in debug mode
            //#if (DEBUG)
            if (DebugMode)
            {
                //args = new[] { "PREBUILDER", "1.2.3.4", "RELEASE" };
                //args = new[] { "PREBUILDER", "1.2.3.4", "BETA" };
                //args = new[] { "PREBUILDER", "1.2.3.4", "BUILD" };
                // args = new[] { "PREBUILDER", "1.2.3.4", "NONE" };
                args = new[] { "PREBUILDER", "READ", "READ" }; // default use existing version/type and set date
                // args = new[] { "PREBUILDER", "READ", "READ", "00000000" }; // resets git subversion and AssemblyConfiguration
                //args = new[] { "" }; // shows help
            }
            //#endif
            if (!args.Any() || args[0].ToUpper().Contains("HELP") || args[0].Contains("?"))
            {
                Console.WriteLine("");
                Console.WriteLine(" CLI RocksmithPreBuild.exe");
                Console.WriteLine("");
                Console.WriteLine(" - Version: " + ProjectVersion());
                Console.WriteLine("   Copyright (C) 2017 CST Developers, Cozy1");
                Console.WriteLine("");
                Console.WriteLine(" - Purpose: FOR DEVELOPER USE ONLY");
                Console.WriteLine("            Updates 'AssemblyInfo.cs' and 'PatchAssemblyVersion.ps1' files");
                Console.WriteLine("");
                Console.WriteLine(" - Syntax:  RocksmithPostBuild.exe [arg0] [arg1] [arg2]");
                Console.WriteLine("            arg0 = 'PREBUILDER' or 'CONVERT'");
                Console.WriteLine("            [arg1] and [arg2] for read/write to 'PatchAssemblyVersion.ps1' file");
                Console.WriteLine("            arg1 = 'READ' [AssemblyVersion Read Mode] ");
                Console.WriteLine("            arg2 = 'READ' [AssemblyConfiguration Read Mode]");
                Console.WriteLine("");
                Console.WriteLine("            arg1 = '2.8.3.0' [AssemblyVersion Write Mode]");
                Console.WriteLine("            arg2 = 'BUILD', 'BETA', 'RELEASE' sets [AssemblyConfiguration] to corresponding string");
                Console.WriteLine("                   'DATE' sets [AssemblyConfiguration] to $env:APPVEYOR_REPO_COMMIT_TIMESTAMP");
                Console.WriteLine("                   'NONE' sets [AssemblyConfiguration] to empty string");
                Console.WriteLine("");
                Console.WriteLine(" - Optional: (write git subversion)");
                Console.WriteLine("            arg3 = '00000000' [AssemblyInformationVersion Write Mode] aka gitsubversion");
                Console.WriteLine("");
                Console.WriteLine(" - Usage:   Run CLI RocksmithPreBuilder.exe with arguments from inside");
                Console.WriteLine("            the VS2010 DEBUG MODE pre-build event in RocksmithToolkitLib");
                Console.WriteLine("            e.g. cmd /c \"RocksmithPreBuild.exe PREBUILD 1.2.3.4 RELEASE\"");
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue");
                Console.Read();

                Environment.Exit(1);
            }

            // convert any VS Project to VS2010
            if (args[0].ToUpper().Contains("CONVERT"))
                ConvertVsProject();

            // check for existence of critical files
            if (!File.Exists(patchAssemblyVersionPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" - <ERROR> Could not find critical file ...");
                Console.WriteLine(patchAssemblyVersionPath);
                Console.ForegroundColor = ConsoleColor.Green;

                Environment.Exit(1);
            }

            if (!File.Exists(gitHeadPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" - <ERROR> Could not find critical file ...");
                Console.WriteLine(gitHeadPath);
                Console.ForegroundColor = ConsoleColor.Green;

                Environment.Exit(1);
            }

            // get gitSubVersion (AssemblyInformationVersion) from in '.git' folders
            Console.WriteLine(" - Reading: " + gitHeadPath);
            var lines = File.ReadAllLines(gitHeadPath).ToList();
            var line = lines.FirstOrDefault(str => str.Contains("ref:"));
            if (!String.IsNullOrEmpty(line))
            {
                var refsFolder = line.Replace("ref: ", "").Replace("/", "\\");
                var masterPath = Path.Combine(appPath, ".git", refsFolder);

                if (!File.Exists(masterPath))
                {
                    ShowHelpfulError(" - <ERROR>: Could not find critical file " + masterPath);

                    Environment.Exit(1);
                }

                //if(DebugMode) Console.WriteLine(" - Reading: " + masterPath);
                lines = File.ReadAllLines(masterPath).ToList();
            }
            else
            {
                ShowHelpfulError(" - <ERROR>: Could not find critical GitSubVersion data ...");

                Environment.Exit(1);
            }

            if (args.Length == 4)
                assemblyInformationVersion = args[3];
            else
                assemblyInformationVersion = lines[0].Substring(0, 8);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" - gitSubVersion [AssemblyInformationVersion]: " + assemblyInformationVersion);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");

            // process (read or write) the PatchAssemblyVersion.ps1 file
            Console.Write(" - Reading: " + patchAssemblyVersionPath);
            lines = File.ReadAllLines(patchAssemblyVersionPath).ToList();
            if (lines.Any())
            {
                // $Assembly_Version = "1.2.3.4" 
                line = lines.Where(str => str.Contains("$Assembly_Version")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if (args[1].ToUpper() == "READ")
                        {
                            assemblyVersion = GetStringInBetween("\"", "\"", line);
                            
                            Console.WriteLine(" - Read $Assembly_Version: " + assemblyVersion);
                        }
                        else
                        {
                            assemblyVersion = args[1];
                            lines[idx] = "$Assembly_Version = \"" + assemblyVersion + "\"";
                            File.WriteAllLines(patchAssemblyVersionPath, lines.ToArray());
                            Console.WriteLine(" - Updated $Assembly_Version: " + assemblyVersion);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" - <ERROR> Could not find '$Assembly_Version' ...");
                        Console.WriteLine(patchAssemblyVersionPath);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                }

                // $Assembly_Configuration = "BETA" 
                line = lines.Where(str => str.Contains("$Assembly_Configuration")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if (args[2].ToUpper() == "READ")
                        {
                            // default AssemblyConfiguration sortable ISO8601 DateTime format (yyyy-MM-ddTHH:mm:ss) 2019-10-22T01:33:36
                            if (line.Contains("APPVEYOR_REPO_COMMIT_TIMESTAMP"))
                                assemblyConfiguration = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);
                            else
                                assemblyConfiguration = GetStringInBetween("\"", "\"", line);

                            Console.WriteLine(" - Read $Assembly_Configuration: " + assemblyConfiguration);
                        }
                        else
                        {
                            // get the appveyor DateTime environmental variable
                            if (args[2].ToUpper() == "DATE")
                                assemblyConfiguration = "$env:APPVEYOR_REPO_COMMIT_TIMESTAMP.Substring(0,19)";
                            else if (args[2].ToUpper() == "NONE")
                                assemblyConfiguration = "";
                            else
                                assemblyConfiguration = (args[2]);

                            if (args[2].ToUpper() == "DATE")
                            {
                                lines[idx] = "$Assembly_Configuration = " + assemblyConfiguration;
                                assemblyConfiguration = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);
                            }
                            else
                                lines[idx] = "$Assembly_Configuration = \"" + assemblyConfiguration + "\"";

                            File.WriteAllLines(patchAssemblyVersionPath, lines.ToArray());
                            Console.WriteLine(" - Updated $Assembly_Configuration: " + assemblyConfiguration);                        
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" - <ERROR> Could not find '$Assembly_Configuration' ...");
                        Console.WriteLine(patchAssemblyVersionPath);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine("");
            }

            // now update AssemblyInfo.cs files
            string[] applicationProjectNames = new string[] { "RocksmithToolkitLib", "RocksmithTookitGUI", "RocksmithToolkitUpdater" };
            foreach (string projectName in applicationProjectNames)
            {
                Console.WriteLine(" - Processing: " + projectName);

                var assemblyInfoPath = Path.Combine(appPath, projectName, "Properties", "AssemblyInfo.cs");
                if (!File.Exists(assemblyInfoPath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" - <ERROR> Could not find AssemblyInfoPath ...");
                    Console.WriteLine(assemblyInfoPath);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("");

                    continue;
                }

                Console.WriteLine(" - Reading: " + assemblyInfoPath);
                lines = File.ReadAllLines(assemblyInfoPath).ToList();
                if (lines.Any())
                {
                    // [assembly: AssemblyVersion("1.2.3.4")]  
                    line = lines.FirstOrDefault(str => str.Contains("[assembly: AssemblyVersion(\""));
                    if (!string.IsNullOrEmpty(line))
                    {
                        var idx = lines.IndexOf(line);
                        if (idx > -1)
                        {
                            lines[idx] = "[assembly: AssemblyVersion(\"" + assemblyVersion + "\")]";
                            File.WriteAllLines(assemblyInfoPath, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(" - Updated AssemblyVersion: " + assemblyVersion);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(" - <ERROR> Could not find 'AssemblyVersion' ...");
                            Console.WriteLine(assemblyInfoPath);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    // [assembly: AssemblyInformationalVersion("00000000")]
                    line = lines.FirstOrDefault(str => str.Contains("[assembly: AssemblyInformationalVersion(\""));
                    if (!string.IsNullOrEmpty(line))
                    {
                        var idx = lines.IndexOf(line);
                        if (idx > -1)
                        {
                            lines[idx] = "[assembly: AssemblyInformationalVersion(\"" + assemblyInformationVersion + "\")]";
                            File.WriteAllLines(assemblyInfoPath, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(" - Updated AssemblyInformationalVersion: " + assemblyInformationVersion);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(" - <ERROR> Could not find 'AssemblyInformationalVersion' ...");
                            Console.WriteLine(assemblyInfoPath);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    // [assembly: AssemblyConfiguration("BETA")]
                    line = lines.Where(str => str.Contains("[assembly: AssemblyConfiguration(\"")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var idx = lines.IndexOf(line);
                        if (idx > -1)
                        {
                            lines[idx] = "[assembly: AssemblyConfiguration(\"" + assemblyConfiguration + "\")]";
                            File.WriteAllLines(assemblyInfoPath, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(" - Updated AssemblyConfiguration: " + assemblyConfiguration);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(" - <ERROR> Could not find 'AssemblyConfiguration' ...");
                            Console.WriteLine(assemblyInfoPath);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

                Console.WriteLine("");
            }

            // write a new VersionInfo.txt file
            Console.WriteLine(" - Writing: " + versionInfoPath);
            using (StreamWriter writer = new StreamWriter(versionInfoPath, false))
            {
                writer.WriteLine(assemblyVersion);
                writer.WriteLine(assemblyInformationVersion);
                writer.WriteLine(assemblyConfiguration);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" - Updated VersionInfo Data ...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");

            // finish up
            Console.WriteLine("RocksmithPostBuild Finished ...");
            Console.WriteLine("Press any key to continue");
            if (DebugMode) Console.Read();

            Environment.Exit(0);
        }


        private static void ConvertVsProject(bool wait = false)
        {
            // convert all project files to VS2010 format
            var path = @".\"; // CAREFUL - ONLY GO UP ONE DIRECTORY
            var convertedCount = 0;
            var fileCount = 0;
            var slnFiles = Directory.EnumerateFiles(path, "*.sln", SearchOption.AllDirectories).ToList();
            fileCount = slnFiles.Count;

            foreach (string slnFile in slnFiles)
            {
                Console.WriteLine(slnFile);
                var lines = File.ReadAllLines(slnFile).ToList();
                if (lines.Count() > 0)
                {
                    var z = lines.FirstOrDefault(str => str.Contains("Format Version 12.00"));
                    if (!string.IsNullOrEmpty(z))
                    {
                        var idx = lines.IndexOf(z);
                        if (idx > -1)
                        {
                            lines[idx] = "Microsoft Visual Studio Solution File, Format Version 11.00";
                            lines[idx + 1] = "# Visual Studio 2010";
                            lines.RemoveAll(m => m.StartsWith("VisualStudioVersion"));
                            lines.RemoveAll(m => m.StartsWith("MinimumVisualStudioVersion"));
                            File.WriteAllLines(slnFile, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Converted :" + Path.GetFileName(slnFile));
                            Console.ForegroundColor = ConsoleColor.Green;
                            convertedCount++;
                        }
                    }
                }
            }

            var csprojFiles = Directory.EnumerateFiles(path, "*.csproj", SearchOption.AllDirectories).ToList();
            fileCount = fileCount + csprojFiles.Count;

            foreach (string csprojFile in csprojFiles)
            {
                Console.WriteLine(csprojFile);
                var lines = File.ReadAllLines(csprojFile).ToList();
                if (lines.Count() > 0)
                {
                    var z = lines.FirstOrDefault(str => str.Contains("Project ToolsVersion=\"12.0\""));
                    if (!string.IsNullOrEmpty(z))
                    {
                        var idx = lines.IndexOf(z);
                        if (idx > -1)
                        {
                            lines[idx] = "<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" ToolsVersion=\"4.0\">";
                            lines.RemoveAll(m => m.StartsWith("<Import Project=\"$(MSBuildExtensionsPath)"));
                            File.WriteAllLines(csprojFile, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Converted :" + Path.GetFileName(csprojFile));
                            Console.ForegroundColor = ConsoleColor.Green;
                            convertedCount++;
                        }
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Converted: " + convertedCount + " of " + fileCount + " files.");
            Console.WriteLine("Done converting");
            Console.WriteLine("");
            Console.WriteLine("Press any key to continue");
            if (DebugMode) Console.Read();

            Environment.Exit(0);
        }

        private static void ShowHelpfulError(string message)
        {
            Console.WriteLine(" - RocksmithPostBuilder:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            if (DebugMode) Console.Read();
        }

        private static string ProjectVersion()
        {
            return String.Format("{0}.{1}.{2}",
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                Assembly.GetExecutingAssembly().GetName().Version.Minor,
                Assembly.GetExecutingAssembly().GetName().Version.Build);
        }

        public static string GetStringInBetween(string strBegin, string strEnd, string strSource)
        {
            string result = "";
            int iIndexOfBegin = strSource.IndexOf(strBegin);
            if (iIndexOfBegin != -1)
            {
                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    result = strSource.Substring(0, iEnd);
                }
            }
            return result;
        }

#if (DEBUG)
        public static bool DebugMode { get { return true; } }
#else
        public static bool DebugMode { get { return false; } }
#endif

    }
}


