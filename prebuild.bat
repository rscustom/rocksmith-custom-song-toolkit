@echo off
COLOR 0A

echo.
echo. - FOR DEVELOPER USE ONLY
echo.
echo. - Purpsose:
echo.   'AssemblyVersion and 'AssemblyConfiguration' are either read
echo.   or written to inside the 'PatchAssemblyVersion.ps1' file.
echo.   The 'AssemblyInfo.cs' 'VersionInfo.txt' files are then updated.
echo.
echo. - Usage:
echo    Only necessary to run if the prebuild event autoupdate feature fails.
echo.   In a command window type 'RocksmithPreBuilder.exe HELP' for more inforation.
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
echo.
START /B /WAIT RocksmithPreBuild.exe PREBUILD READ READ
echo.

pause

@echo on