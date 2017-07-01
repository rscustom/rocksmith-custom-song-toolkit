cls
@echo off

SET CurrentDate=%date:~-4,4%-%date:~-10,2%-%date:~-7,2%
SET CurrentTIme=%time:~-11,2%%time:~-8,2%%time:~-5,2%
COLOR 0A

echo.
echo. The included prebuild.bat may not work on some systems.  
echo. Use this batch before running VS2010 to make a copy of the 
echo. ToolkitVersion.cs_dist file and save it as ToolkitVersion.cs
echo.
echo. This batch installs standardized CST-2014-04-09.vssettings to VS2010
echo. ALL pull request and commits to CST must adhear to these settings
echo.
echo. This batch backups your current VS2010 settings to *.vssettings.bak file
echo.
echo. Read the instructions included in the CONTRIBUTING.md file.
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
echo.
echo f | XCOPY /y "%UserPROFILE%\My Documents\Visual Studio 2010\Settings\CurrentSettings.vssettings" "%UserPROFILE%\My Documents\Visual Studio 2010\Settings\Currentsettings_%CurrentDate%_T%CurrentTime%.vssettings.bak" 

echo.
echo f | XCOPY /y .\CST-2014-04-09.vssettings "%UserPROFILE%\Documents\Visual Studio 2010\Settings\Currentsettings.vssettings" 

echo.
START /B /WAIT RocksmithPreBuilder.exe "PREBUILD" "READ" "READ"

.\contributing.md >> CON

@echo on

