using System;
using System.Linq;
using System.Reflection;


namespace RocksmithToolkitLib
{
    public static class ToolkitVersion
    {
        // NOTES FOR DEVS:
        // the CLI RocksmithDevBuilder.exe is run from VS pre-build events
        // the 'ToolkitVersion.cs', all 'AssemblyInfo.cs' and 
        // VersionInfo.txt files will be updated automatically

        // manually update 'assemblyVersion' and 'releaseType' variables here
        public static string assemblyVersion = "2.8.3.1";
        public static string releaseType = "BETA";

        // NOTE FOR DEVS: additionally the gitSubVersion below is automatically
        // updated by appveyor.yml and subsequently by CLI RocksmithDevBuilder.exe
		public static string gitSubVersion = "27e5af87";

        public static string version
        {
            get
            {
                // experimentation with alt methods to pull data from ExecutingAssembly
                var av = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var gsb = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();
    
                return String.Format("{0}-{1} {2}", assemblyVersion, gitSubVersion, releaseType);
            }
        }


    }
}
