using System;
using System.Linq;
using System.Reflection;
using System.IO;


namespace RocksmithToolkitLib
{
    public static class ToolkitVersion
    {
        // NOTE FOR DEVS: 
        // assembly variables are automatically updated by appveyor.yml and PatchAssemblyVersion.ps1
        // DO NOT MAKE ANY CHANGES HERE
        // the 'AssemblyVersion' and 'AssemblyConfiguration' values must be manually edited in 
        // the 'PatchAssemblyVersion.ps1' file prior to committing major revisions/releases
        
        // assemblyVersion e.g. "2.8.3.1"
        public static string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
        // assemblyInformationVersion (aka gitSubVersion) e.g. "ce57ebea"
        public static string AssemblyInformationVersion = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();
        
        // assemblyConfigurate e.g. "BETA" or blank
        public static string AssemblyConfiguration = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString();

        public static string version
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
    }
}
