@echo off
COLOR 0A

echo.
echo. - FOR DEVELOPER USE ONLY
echo.
echo. - Purpsose:
echo.   'assemblyVersion and 'releaseType' are read from the ToolkitVersion.cs 
echo.   'assemblyVersion' will be written to the 'PatchAssemblyVersion.ps1' 
echo.   which is used use by appveyor.yml to autobuid the toolkit solution
echo.   'ToolkitVersion.cs' and all 'AssemblyInfo.cs' files will be updated
echo.   'VersionInfo.txt' file will be written
echo.
echo. - Usage:
echo    Only necessary to run if the prebuild event autoupdate feature fails
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
echo.
@echo on


START /B /WAIT RocksmithDevBuilder.exe TOOLKITVERS WAIT
