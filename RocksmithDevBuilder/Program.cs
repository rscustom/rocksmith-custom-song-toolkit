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
    internal class Program
    {
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
            if (DebugMode) Console.SetWindowPosition(0, 0);
            if (DebugMode) Console.SetWindowSize(90, 35);
            if (DebugMode) Console.BackgroundColor = ConsoleColor.Black;
            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;

            // feed the CLI some data when working in debug mode
            if (DebugMode)
                args = new[] { "TOOLKITVER", "WAIT" };

            if (!args.Any() || args.Length != 2 || args[0].ToUpper().Contains("HELP") || args[0].Contains("?"))
            {
                if (DebugMode) Console.WriteLine("");
                if (DebugMode) Console.WriteLine(" CLI RocksmithDevBuilder.exe");
                if (DebugMode) Console.WriteLine("");
                if (DebugMode) Console.WriteLine(" - Version: " + ProjectVersion());
                if (DebugMode) Console.WriteLine("   Copyright (C) 2017 CST Developers, Cozy1");
                if (DebugMode) Console.WriteLine("");
                if (DebugMode) Console.WriteLine(" - Purpose: FOR DEVELOPER USE ONLY");
                if (DebugMode) Console.WriteLine("            Updates 'ToolkitVersion.cs' and 'AssemblyInfo.cs' file variables");
                if (DebugMode) Console.WriteLine("");
                if (DebugMode) Console.WriteLine(" - Syntax:  RocksmithPostBuilder.exe [arg0] [arg1]");
                if (DebugMode) Console.WriteLine("            arg0 = 'TOOLKITVERS' or 'CONVERT'");
                if (DebugMode) Console.WriteLine("            arg1 = 'WAIT' or 'NOWAIT'");
                if (DebugMode) Console.WriteLine("");
                if (DebugMode) Console.WriteLine(" - Usage:   Run CLI batch from the VS2010 pre-build event in RocksmithToolkitLib");
                if (DebugMode) Console.WriteLine("");
                if (wait) if (DebugMode) Console.ReadLine();
                Environment.Exit(-1);
                return;
            }

            if (args[1].ToUpper().Equals("NOWAIT"))
                wait = false;

            // alternate CLI usage ... Easter Egg
            if (args[0].ToUpper().Contains("CONVERT"))
                ConvertVsProject(wait);

            // check for existence of critical files
            if (!File.Exists(gitHeadPath))
            {
                if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                if (DebugMode) Console.WriteLine(" - <ERROR> Could not find critical file ...");
                if (DebugMode) Console.WriteLine(gitHeadPath);
                if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                Environment.Exit(-1);
                return;
            }

            if (!File.Exists(patchAssemblyVersionPath))
            {
                if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                if (DebugMode) Console.WriteLine(" - <ERROR> Could not find critical file ...");
                if (DebugMode) Console.WriteLine(patchAssemblyVersionPath);
                if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                Environment.Exit(-1);
                return;
            }

            // get assemblyVersion and releaseType from ToolkitVersion.cs
            if (!File.Exists(toolkitVersionPath))
            {
                if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                if (DebugMode) Console.WriteLine(" - <ERROR> Could not find critical file ...");
                if (DebugMode) Console.WriteLine(toolkitVersionPath);
                if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                Environment.Exit(-1);
                return;
            }

            // get gitSubVersion from .git folders
            if (DebugMode) Console.WriteLine(" - Reading: " + gitHeadPath);
            var lines = File.ReadAllLines(gitHeadPath).ToList();
            var line = lines.Where(str => str.Contains("ref:")).FirstOrDefault();
            if (!String.IsNullOrEmpty(line))
            {
                var refsFolder = line.Replace("ref: ", "").Replace("/", "\\");
                var masterPath = Path.Combine(appPath, ".git", refsFolder);

                if (!File.Exists(masterPath))
                {
                    ShowHelpfulError(" - <ERROR>: Could not find critical file " + masterPath);
                    Environment.Exit(-1);
                    return;
                }

                //if(DebugMode) Console.WriteLine(" - Reading: " + masterPath);
                lines = File.ReadAllLines(masterPath).ToList();
            }
            else
            {
                ShowHelpfulError(" - <ERROR>: Could not find critical GitSubVersion data ...");
                Environment.Exit(-1);
                return;
            }

            gitSubVersion = lines[0].Substring(0, 8);
            if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
            if (DebugMode) Console.WriteLine(" - gitSubVersion: " + gitSubVersion);
            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
            if (DebugMode) Console.WriteLine("");

            // get assemblyVersion and releaseType from ToolkitVersion.cs file
            if (DebugMode) Console.WriteLine(" - Reading: " + toolkitVersionPath);
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
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
                        if (DebugMode) Console.WriteLine(" - assemblyVersion: " + assemblyVersion);
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                        if (DebugMode) Console.WriteLine(" - <ERROR> Could not find assemblyVersion ...");
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

                line = lines.Where(str => str.Contains("public static string releaseType")).FirstOrDefault();
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = lines.IndexOf(line);
                    if (idx > -1)
                    {
                        releaseType = GetStringInBetween("\"", "\"", line);
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
                        if (DebugMode) Console.WriteLine(" - releaseType: " + releaseType);
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                        if (DebugMode) Console.WriteLine(" - <ERROR> Could not find 'releaseType' ...");
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
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
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
                        if (DebugMode) Console.WriteLine(" - Updated gitSubVersion: " + gitSubVersion);
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                        if (DebugMode) Console.WriteLine(" - <ERROR> Could not find 'gitSubVersion' ...");
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }

                    if (DebugMode) Console.WriteLine("");
                }
            }
            else
            {
                ShowHelpfulError(" - <ERROR>: Could not read critical data " + toolkitVersionPath);
                Environment.Exit(-1);
                return;
            }

            foreach (string projectName in applicationProjectNames)
            {
                if (DebugMode) Console.WriteLine(" - Updating: " + projectName);

                var assemblyInfoPath = Path.Combine(appPath, projectName, "Properties", "AssemblyInfo.cs");
                if (!File.Exists(assemblyInfoPath))
                {
                    if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                    if (DebugMode) Console.WriteLine(" - <ERROR> Could not find AssemblyInfoPath ...");
                    if (DebugMode) Console.WriteLine(assemblyInfoPath);
                    if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    if (DebugMode) Console.WriteLine("");

                    continue;
                }

                if (DebugMode) Console.WriteLine(" - AssemblyInfo: " + assemblyInfoPath);
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
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
                            if (DebugMode) Console.WriteLine(" - Updated AssemblyVersion: " + assemblyVersion);
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                            if (DebugMode) Console.WriteLine(" - <ERROR> Could not find 'AssemblyVersion' ...");
                            if (DebugMode) Console.WriteLine(assemblyInfoPath);
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
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
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
                            if (DebugMode) Console.WriteLine(" - Updated AssemblyInformationalVersion: " + gitSubVersion);
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                            if (DebugMode) Console.WriteLine(" - <ERROR> Could not find 'AssemblyInformationalVersion' ...");
                            if (DebugMode) Console.WriteLine(assemblyInfoPath);
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                        }

                        if (DebugMode) Console.WriteLine("");
                    }
                }
            }

            // update the PatchAssemblyVersion.ps1 file
            if (DebugMode) Console.WriteLine(" - PatchAssemblyVersion: " + patchAssemblyVersionPath);
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
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
                        if (DebugMode) Console.WriteLine(" - Updated $Assembly_Version: " + assemblyVersion);
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
                        if (DebugMode) Console.WriteLine(" - <ERROR> Could not find '$Assembly_Version' ...");
                        if (DebugMode) Console.WriteLine(patchAssemblyVersionPath);
                        if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

                if (DebugMode) Console.WriteLine("");
            }

            // write a new VersionInfo.txt file
            using (StreamWriter writer = new StreamWriter(versionInfoPath, false))
            {
                writer.WriteLine(assemblyVersion);
                writer.WriteLine(gitSubVersion);
                writer.WriteLine(releaseType);
            }

            if (DebugMode) Console.WriteLine(" - VersionInfo: " + versionInfoPath);
            if (DebugMode) Console.ForegroundColor = ConsoleColor.Cyan;
            if (DebugMode) Console.WriteLine(" - Updated VersionInfo ...");
            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
            if (DebugMode) Console.WriteLine("");

            // finish up
            if (DebugMode) Console.WriteLine("RocksmithPostBuild Finished ...");
            if (DebugMode) Console.WriteLine("Press any key to continue");
            if (wait) if (DebugMode) Console.ReadLine();

            Environment.Exit(0);
            return;
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
                if (DebugMode) Console.WriteLine(slnFile);
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
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Red;
                            if (DebugMode) Console.WriteLine("Converted :" + Path.GetFileName(slnFile));
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                            convertedCount++;
                        }
                    }
                }
            }

            var csprojFiles = Directory.EnumerateFiles(path, "*.csproj", SearchOption.AllDirectories).ToList();
            fileCount = fileCount + csprojFiles.Count;

            foreach (string csprojFile in csprojFiles)
            {
                if (DebugMode) Console.WriteLine(csprojFile);
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
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Red;
                            if (DebugMode) Console.WriteLine("Converted :" + Path.GetFileName(csprojFile));
                            if (DebugMode) Console.ForegroundColor = ConsoleColor.Green;
                            convertedCount++;
                        }
                    }
                }
            }

            if (DebugMode) Console.WriteLine("");
            if (DebugMode) Console.WriteLine("Converted: " + convertedCount + " of " + fileCount + " files.");
            if (DebugMode) Console.WriteLine("Done converting");
            if (DebugMode) Console.WriteLine("");
            if (DebugMode) Console.WriteLine("Press any key to continue");
            if (wait) if (DebugMode) Console.ReadLine();
            Environment.Exit(0);
            return;
        }

        private static void ShowHelpfulError(string message, bool wait = false)
        {
            if (DebugMode) Console.WriteLine(" - RocksmithPostBuilder:");
            if (DebugMode) Console.ForegroundColor = ConsoleColor.Yellow;
            if (DebugMode) Console.WriteLine(message);
            if (wait) if (DebugMode) Console.ReadLine();
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
