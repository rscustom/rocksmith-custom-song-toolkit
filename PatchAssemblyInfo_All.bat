REM adds git subversion commit references to all AssemblyInfo.cs
REM run this batch after AppVeyor builds

@echo off

setlocal enabledelayedexpansion

if errorlevel 1 goto BuildEventFailed

call PatchAssemblyInfo.bat ".\" ".\RocksmithToolkitLib\ToolkitVersion.cs"
call PatchAssemblyInfo.bat ".\" ".\RocksmithToolkitLib\Properties\AssemblyInfo.cs"
call PatchAssemblyInfo.bat ".\" ".\RocksmithToolkitUpdater\Properties\AssemblyInfo.cs"
call PatchAssemblyInfo.bat ".\" ".\RocksmithTookitGUI\Properties\AssemblyInfo.cs"

echo Done
::pause

endlocal
exit /b 0

:BuildEventFailed
echo Pre-Build Event Failed in prebuild.bat file
pause
