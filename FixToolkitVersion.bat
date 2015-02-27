cls
@echo off
echo.The included Prebuild may not work on some systems.  
echo.Use this bat before running VS2010 to make a copy of the 
echo.ToolkitVersion.cs_dist file and save it as ToolkitVersion.cs
echo.
echo.Read the instructions included in the CONTRIBUTING.md file.
echo.

@echo on
call prebuild.bat . RocksmithToolkitLib\ToolkitVersion.cs
@echo off

.\contributing.md >> CON

echo.
pause
@echo on

