REM the AssemblyVersion and ReleaseType are read from the ToolkitVersion.cs 
REM the AssemblyVersion will be written to the 'PatchAssemblyVersion.ps1' file for use by AppVeyor
REM the 'ToolkitVersion.cs' and all 'AssemblyInfo.cs' files will be updated
REM the 'VersionInfo.txt' file will be written

START /B /WAIT RocksmithDevBuilder.exe TOOLKITVERS WAIT
