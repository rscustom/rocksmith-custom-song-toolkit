using System;
using System.Reflection;


namespace RocksmithToolkitLib
{
    public static class ToolkitVersion
    {
        // NOTES:
        // The pre-build event is not working 
        // so manually run the RocksmithToolkitLibPreBuild.bat after git commit
        // TODO: FIXME the CLI RocksmithDevBuilder.exe is run from VS pre-build events
        // the 'ToolkitVersion.cs' and all 'AssemblyInfo.cs' files will be updated automatically

        // as necessary, manually update these variables prior to pushing a new git commit
        // it is no longer necessary to manually update the 'AssemblyInfo.cs' files
        public static string assemblyVersion = "2.8.3.1";
        public static string releaseType = "BETA";
       
        // it is is not necessary to manually update the gitSubVersion data
        // gitSubVersion will be automatically updated by CLI RocksmithDevBuilder.exe
		public static string gitSubVersion = "9605d7f3";

        public static string version
        {
            get
            {
                return String.Format("{0}-{1} {2}", assemblyVersion, gitSubVersion, releaseType);
            }
        }
    }
}
