@echo off
setlocal enabledelayedexpansion

if errorlevel 1 goto BuildEventFailed

set batchpath=%~dp0
echo Current batch path: %batchpath%
cd %batchpath%

set solution=%~1
set toolkitver=%~2
echo Solution Path from Command Line: %solution%
echo Toolkit Version Path from Command Line: %toolkitver%

REM args for testing
if "%solution%"=="" (
set solution=.\
)

REM args for testing
if "%toolkitver%"=="" (
rem set toolkitver=.\RocksmithToolkitLib\ToolkitVersion.cs
rem set toolkitver=.\RocksmithToolkitLib\Properties\AssemblyInfo.cs
rem set toolkitver=.\RocksmithToolkitUpdater\Properties\AssemblyInfo.cs
set toolkitver=.\RocksmithTookitGUI\Properties\AssemblyInfo.cs
)

set toolkitverdist=%toolkitver%_dist

echo solution %solution%
echo toolkitver %toolkitver%
echo toolkitverdist %toolkitverdist%
echo Copying %toolkitverdist% 
echo To %toolkitver%

copy %toolkitverdist% %toolkitver%

echo Checking .git\HEAD exists ...
:: get git commit version from .git\refs\heads\master file
:: github commit version script

if exist %solution%\.git\HEAD (
	echo Reading .git\HEAD ...
	set /p head=<"%solution%\.git\HEAD"
	if "!head:~0,4!" == "ref:" (
		set master=.git\!head:~5!
		if exist "%solution%\.git\!head:~5!" set /p commit=<"%solution%\.git\!head:~5!"
	) else (
		set commit=!head!
	)
	if not "!commit!" == ""	(
		echo Found commit: !commit!
		set newrev=!commit:~0,8!
		echo newrev !newrev!
				for %%a in (%solution%!master!) do set newrevdate=%%~ta
				echo newrevdate !newrevdate!
	) else echo Unable to find commit ...
)

set oldrev=00000000

::pause

REM this has been depricated does not seem to be needed with revised scripts
::*.cs files in Unicode, required for VS2010 Pre-Build Event to work with WinXP
::chcp 65001>nul not reliable on WinXP SP3
::alt work around
::(
::chcp 65001
::cmd /c type myfile.txt
::chcp 850
::)

echo Replacing %oldrev% in tempfile.txt with %newrev% ...
::pause

::git version replacement script
for /f "tokens=* delims==" %%i in (%toolkitverdist%) do (
	set str=%%i
	set newstr=!str:%oldrev%=%newrev%!
	echo !newstr!>>tempfile.txt
)

::pause
echo Moving tempfile.txt to new %toolkitver% ...
move /y tempfile.txt "%toolkitver%"

echo Creating VersionInfo.txt ...
echo %newrev% > VersionInfo.txt
echo Done

::pause

endlocal
exit /b 0

:BuildEventFailed
echo Pre-Build Event Failed in prebuild.bat file
pause
