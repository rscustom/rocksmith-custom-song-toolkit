using System;
using System.Linq;
using System.Reflection;
using System.IO;


namespace RocksmithToolkitLib
{
    public static class ToolkitVersion
    {
        // DEVNOTE: 
        // assembly variables are automatically updated by appveyor.yml and PatchAssemblyVersion.ps1
        //
        // DO NOT MAKE ANY CHANGES HERE ...
        //
        // Edit the Pre-build Commands for the RocksmithToolkitLib 
        // For directions see the REM comments in the RocksmithToolkitLib Pre-build Commands
        // then RocksmithPreBuild.exe takes care of updating Assembly.cs files and AppVeyor versioning information
        //
        // Alternatively prior to committing major revisions/releases ...
        // the 'AssemblyVersion' and 'AssemblyConfiguration' values must be edited in 
        // the 'PatchAssemblyVersion.ps1' file as needed.

        // assemblyVersion e.g. "1.2.3.4"
        public static string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // assemblyInformationVersion (aka gitSubVersion) e.g. "ce57ebea"
        public static string AssemblyInformationVersion = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();

        // assemblyConfigurate e.g. "BETA" or blank
        public static string AssemblyConfiguration = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";

        public static string RSTKGuiVersion
        {
            get
            {
                return String.Format("{0}-{1} {2}", AssemblyVersion, AssemblyInformationVersion, AssemblyConfiguration);
            }
        }

        public static void UpdateVersionInfoFile()
        {
            // write a new VersionInfo.txt file
            var appExe = Assembly.GetExecutingAssembly().Location;
            var appPath = Path.GetDirectoryName(appExe);
            var versionInfoPath = Path.Combine(appPath, "VersionInfo.txt");

            using (StreamWriter writer = new StreamWriter(versionInfoPath, false))
            {
                writer.WriteLine(AssemblyVersion);
                writer.WriteLine(AssemblyInformationVersion);
                writer.WriteLine(AssemblyConfiguration);
            }
        }

        public static string RSTKLibVersion()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath("RocksmithToolkitLib.dll"));
            var assemblyVersion = assembly.GetName().Version.ToString();
            var assemblyInformationVersion = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();
            var assemblyConfiguration = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";

            return String.Format("{0}-{1} {2}", assemblyVersion, assemblyInformationVersion, assemblyConfiguration);
        }

        public static string RSTKUpdaterVersion()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath("RocksmithToolkitUpdater.exe"));
            var assemblyVersion = assembly.GetName().Version.ToString();
            var assemblyInformationVersion = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();
            var assemblyConfiguration = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";

            return String.Format("{0}-{1} {2}", assemblyVersion, assemblyInformationVersion, assemblyConfiguration);
        }

    }
}
