using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocksmithDevBuilder
{
    class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // common variables are here
            string[] applicationProjectNames = new string[] { "RocksmithToolkitLib", "RocksmithTookitGUI", "RocksmithToolkitUpdater" };
            var gitSubVersion = "00000000";
            var assemblyVersion = "0.0.0.0";
            var releaseType = "BETA";

            var appExe = Assembly.GetExecutingAssembly().Location;
            var appPath = Path.GetDirectoryName(appExe);
            var parentPath = Path.GetDirectoryName(appPath);
            var gitHeadPath = Path.Combine(appPath, ".git", "HEAD");
            var patchAssemblyVersionPath = Path.Combine(appPath, "PatchAssemblyVersion.ps1");
            var toolkitVersionPath = Path.Combine(appPath, "RocksmithToolkitLib", "ToolkitVersion.cs");
            var versionInfoPath = Path.Combine(appPath, "VersionInfo.txt");
            var wait = true;

            // set CLI appearance
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            // commented out ... causing error if exe is run from the toolkit prebuild event
            // Console.SetWindowPosition(0, 0);
            // Console.SetWindowSize(90, 35);

            // feed the CLI some data when working in debug mode
            if (DebugMode)
                args = new[] { "TOOLKITVER", "WAIT" };

            if (!args.Any() || args.Length != 2 || args[0].ToUpper().Contains("HELP") || args[0].Contains("?"))
            {
                Console.WriteLine("");
                Console.WriteLine(" CLI RocksmithDevBuilder.exe");
                Console.WriteLine("");
                Console.WriteLine(" - Version: " + ProjectVersion());
                Console.WriteLine("   Copyright (C) 2017 CST Developers, Cozy1");
                Console.WriteLine("");
                Console.WriteLine(" - Purpose: FOR DEVELOPER USE ONLY");
                Console.WriteLine("            Updates 'ToolkitVersion.cs' and 'AssemblyInfo.cs' file variables");
                Console.WriteLine("");
                Console.WriteLine(" - Syntax:  RocksmithPostBuilder.exe [arg0] [arg1]");
                Console.WriteLine("            arg0 = 'TOOLKITVERS' or 'CONVERT'");
                Console.WriteLine("            arg1 = 'WAIT' or 'NOWAIT'");
                Console.WriteLine("");
                Console.WriteLine(" - Usage:   Run CLI batch from the VS2010 pre-build event in RocksmithToolkitLib");
                Console.WriteLine("");
                if (wait) Console.Read();
                
                Environment.Exit(1);
            }

            if (args[1].ToUpper().Equals("NOWAIT"))
                wait = false;

            // alternate CLI usage ... Easter Egg
            if (args[0].ToUpper().Contains("CONVERT"))
                ConvertVsProject(wait);

            // check for existence of critical files
            if (!File.Exists(gitHeadPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" - <ERROR> Could not find critical file ...");
                Console.WriteLine(gitHeadPath);
                Console.ForegroundColor = ConsoleColor.Green;
                
                Environment.Exit(1);
            }

            if (!File.Exists(patchAssemblyVersionPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" - <ERROR> Could not find critical file ...");
                Console.WriteLine(patchAssemblyVersionPath);
                Console.ForegroundColor = ConsoleColor.Green;
                
                Environment.Exit(1);
            }

            // get assemblyVersion and releaseType from ToolkitVersion.cs
            if (!File.Exists(toolkitVersionPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" - <ERROR> Could not find critical file ...");
                Console.WriteLine(toolkitVersionPath);
                Console.ForegroundColor = ConsoleColor.Green;
                
                Environment.Exit(1);
            }

            // get gitSubVersion from .git folders
            Console.WriteLine(" - Reading: " + gitHeadPath);
            var lines = File.ReadAllLines(gitHeadPath).ToList();
            var line = lines.Where(str => str.Contains("ref:")).FirstOrDefault();
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

            gitSubVersion = lines[0].Substring(0, 8);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" - gitSubVersion: " + gitSubVersion);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");

            // get assemblyVersion and releaseType from ToolkitVersion.cs file
            Console.WriteLine(" - Reading: " + toolkitVersionPath);
            lines = File.ReadAllLines(toolkitVersionPath).ToList();
            if (lines.Any())
            {
                line = lines.Where(str => str.Contains("public static string assemblyVersion")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        assemblyVersion = GetStringInBetween("\"", "\"", line);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(" - assemblyVersion: " + assemblyVersion);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" - <ERROR> Could not find assemblyVersion ...");
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

                line = lines.Where(str => str.Contains("public static string releaseType")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        releaseType = GetStringInBetween("\"", "\"", line);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(" - releaseType: " + releaseType);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" - <ERROR> Could not find 'releaseType' ...");
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

                line = lines.Where(str => str.Contains("public static string gitSubVersion")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        lines[idx] = "\t\tpublic static string gitSubVersion = \"" + gitSubVersion + "\";";
                        File.WriteAllLines(toolkitVersionPath, lines.ToArray());
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(" - Updated gitSubVersion: " + gitSubVersion);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" - <ERROR> Could not find 'gitSubVersion' ...");
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    Console.WriteLine("");
                }
            }
            else
            {
                ShowHelpfulError(" - <ERROR>: Could not read critical data " + toolkitVersionPath);
                
                Environment.Exit(1);
            }

            foreach (string projectName in applicationProjectNames)
            {
                Console.WriteLine(" - Updating: " + projectName);

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

                Console.WriteLine(" - AssemblyInfo: " + assemblyInfoPath);
                lines = File.ReadAllLines(assemblyInfoPath).ToList();
                if (lines.Any())
                {
                    // [assembly: AssemblyVersion("2.3.8.1")]  
                    line = lines.Where(str => str.Contains("[assembly: AssemblyVersion(\"")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var idx = lines.IndexOf(line);
                        if (idx > -1)
                        {
                            lines[idx] = "[assembly: AssemblyVersion(\"" + assemblyVersion + "\")]";
                            File.WriteAllLines(assemblyInfoPath, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(" - Updated AssemblyVersion: " + assemblyVersion);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(" - <ERROR> Could not find 'AssemblyVersion' ...");
                            Console.WriteLine(assemblyInfoPath);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                    }

                    // [assembly: AssemblyInformationalVersion("9605d7f3")]
                    line = lines.Where(str => str.Contains("[assembly: AssemblyInformationalVersion(\"")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var idx = lines.IndexOf(line);
                        if (idx > -1)
                        {
                            lines[idx] = "[assembly: AssemblyInformationalVersion(\"" + gitSubVersion + "\")]";
                            File.WriteAllLines(assemblyInfoPath, lines.ToArray());
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(" - Updated AssemblyInformationalVersion: " + gitSubVersion);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(" - <ERROR> Could not find 'AssemblyInformationalVersion' ...");
                            Console.WriteLine(assemblyInfoPath);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }

                        Console.WriteLine("");
                    }
                }
            }

            // update the PatchAssemblyVersion.ps1 file
            Console.WriteLine(" - PatchAssemblyVersion: " + patchAssemblyVersionPath);
            lines = File.ReadAllLines(patchAssemblyVersionPath).ToList();
            if (lines.Any())
            {
                // $Assembly_Version = "0.0.0.0" 
                line = lines.Where(str => str.Contains("$Assembly_Version")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        lines[idx] = "$Assembly_Version = \"" + assemblyVersion + "\"";
                        File.WriteAllLines(patchAssemblyVersionPath, lines.ToArray());
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(" - Updated $Assembly_Version: " + assemblyVersion);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" - <ERROR> Could not find '$Assembly_Version' ...");
                        Console.WriteLine(patchAssemblyVersionPath);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

                Console.WriteLine("");
            }

            // write a new VersionInfo.txt file
            using (StreamWriter writer = new StreamWriter(versionInfoPath, false))
            {
                writer.WriteLine(assemblyVersion);
                writer.WriteLine(gitSubVersion);
                writer.WriteLine(releaseType);
            }

            Console.WriteLine(" - VersionInfo: " + versionInfoPath);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" - Updated VersionInfo ...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");

            // finish up
            Console.WriteLine("RocksmithPostBuild Finished ...");
            Console.WriteLine("Press any key to continue");
            if (wait) Console.Read();
            
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
                    var z = lines.Where(str => str.Contains("Format Version 12.00")).FirstOrDefault();
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
                    var z = lines.Where(str => str.Contains("Project ToolsVersion=\"12.0\"")).FirstOrDefault();
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
            if (wait) Console.Read();
            
            Environment.Exit(0);
        }

        private static void ShowHelpfulError(string message, bool wait = false)
        {
            Console.WriteLine(" - RocksmithPostBuilder:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            if (wait) Console.Read();
        }

        private static string ProjectVersion()
        {
            return String.Format("{0}.{1}.{2}", Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor, Assembly.GetExecutingAssembly().GetName().Version.Build);
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
