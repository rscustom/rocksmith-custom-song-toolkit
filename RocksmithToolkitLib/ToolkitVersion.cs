using System;
using System.Linq;
using System.Reflection;


namespace RocksmithToolkitLib
{
    public static class ToolkitVersion
    {
        // NOTES:
        // The pre-build event 'RocksmithDevBuilder.exe TOOLKITVER NOWAIT' is not working yet
        // so need to manually run the RocksmithToolkitLibPreBuild.bat before and after git commit
        // TODO: FIXME the CLI RocksmithDevBuilder.exe is run from VS pre-build events
        // the 'ToolkitVersion.cs' and all 'AssemblyInfo.cs' files will be updated automatically

        // manually update 'assemblyVersion' and 'releaseType' variables here
        public static string assemblyVersion = "2.8.3.1";
        public static string releaseType = "BETA";

        // it is is not necessary to manually update the gitSubVersion data
        // gitSubVersion will be automatically updated by appveyor.yml and CLI RocksmithDevBuilder.exe
		public static string gitSubVersion = "4885705d";

        public static string version
        {
            get
            {
                return String.Format("{0}-{1} {2}", assemblyVersion, gitSubVersion, releaseType);
            }
        }

        // alt methods to pull data from ExecutingAssembly
        public static string av = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string gsb = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().FirstOrDefault().InformationalVersion.ToString();

    }
}
