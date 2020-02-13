using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Security.Authentication;
using RocksmithToolkitLib.Extensions;
using System.Diagnostics;
using System.Globalization;
using System.Drawing;


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

        // assemblyConfigurate e.g. "BUILD", "BETA", "RELEASE", or "" (blank) depending on RocksmithPreBuild.exe usage
        public static string AssemblyConfiguration = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";

        public static string RSTKGuiVersion
        {
            get
            {
                if (GeneralExtension.IsInDesignMode)
                    return String.Format("{0}-{1} {2}", AssemblyVersion, AssemblyInformationVersion, "DEBUG").Trim();

                return String.Format("{0}-{1}", AssemblyVersion, AssemblyInformationVersion).Trim();
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

            return String.Format("{0}-{1}", assemblyVersion, assemblyInformationVersion).Trim();
        }

        public static string RSTKUpdaterVersion()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath("RocksmithToolkitUpdater.exe"));
            var assemblyVersion = assembly.GetName().Version.ToString();
            var assemblyInformationVersion = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();
            var assemblyConfiguration = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";

            return String.Format("{0}-{1}", assemblyVersion, assemblyInformationVersion).Trim();
        }
        
        public static bool IsRSTKLibValid(double shelfLifeDays = 180)
        {
            var assembly = Assembly.LoadFile(typeof(RocksmithToolkitLib.ToolkitVersion).Assembly.Location);
            var assemblyConfiguration = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";

            // check if AssemblyConfiguration contains a parsable DateTime
            DateTime temp;
            if (!DateTime.TryParse(assemblyConfiguration, out temp))
                return false;

            // working with UTC to avoid regional DateTime issues
            DateTime dtuAssemblyConfig = DateTime.Parse(assemblyConfiguration, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var dtuNow = DateTime.UtcNow;
            var dtuRemaining = dtuAssemblyConfig.AddDays(shelfLifeDays) - dtuNow;

            if (dtuRemaining.Days < 0)
                return false;

            return true;
        }
    }

    public static class Startup
    {
        //  hackery used to create class library entry point
        public static void Start()
        {
            if (!ToolkitVersion.IsRSTKLibValid())
            {
                // throw new ApplicationException("This version of RocksmithToolkitLib.dll has expired.  " + Environment.NewLine +
                //    "Please download and install the latest toolkit library.  " + Environment.NewLine);

                var diaMsg = "This version of RocksmithToolkitLib.dll is no longer supported." + Environment.NewLine +
                             "Please download and install the latest version of the toolkit.";
                BetterDialog2.ShowDialog(diaMsg, "Time To Update ...", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Warning.Handle), "WARNING ...", 0, 150);
            }
        }
    }

}
