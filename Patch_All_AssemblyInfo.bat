REM adds git subversion commit references to all AssemblyInfo.cs

@echo off

setlocal enabledelayedexpansion

if errorlevel 1 goto BuildEventFailed

call Patch_AssemblyInfo.bat ".\" ".\RocksmithToolkitLib\ToolkitVersion.cs"
call Patch_AssemblyInfo.bat ".\" ".\RocksmithToolkitLib\Properties\AssemblyInfo.cs"
call Patch_AssemblyInfo.bat ".\" ".\RocksmithToolkitUpdater\Properties\AssemblyInfo.cs"
call Patch_AssemblyInfo.bat ".\" ".\RocksmithTookitGUI\Properties\AssemblyInfo.cs"

echo Done
pause

endlocal
exit /b 0

:BuildEventFailed
echo Pre-Build Event Failed in prebuild.bat file
pause
