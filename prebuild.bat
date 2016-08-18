REM adds git subversion commit references to all AssemblyInfo.cs

@echo off

setlocal enabledelayedexpansion

if errorlevel 1 goto BuildEventFailed

call UpdateGitSvn_All.bat ".\" ".\RocksmithToolkitLib\ToolkitVersion.cs"
call UpdateGitSvn_All.bat ".\" ".\RocksmithToolkitLib\Properties\AssemblyInfo.cs"
call UpdateGitSvn_All.bat ".\" ".\RocksmithToolkitUpdater\Properties\AssemblyInfo.cs"
call UpdateGitSvn_All.bat ".\" ".\RocksmithTookitGUI\Properties\AssemblyInfo.cs"

echo Done
pause

endlocal
exit /b 0

:BuildEventFailed
echo Pre-Build Event Failed in prebuild.bat file
pause
