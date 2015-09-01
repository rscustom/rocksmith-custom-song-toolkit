@echo off
setlocal enabledelayedexpansion

if errorlevel 1 goto BuildEventFailed

set solution=%~1
set toolkitver=%~2
::echo Solution Path: %solution%
::echo Toolkit Version Path: %toolkitver%

if "%solution%"=="" (
set solution=.\
)

if "%toolkitver%"=="" (
set toolkitver=.\RocksmithToolkitLib\ToolkitVersion.cs
)

set toolkitverdist=%toolkitver%_dist

echo solution %solution%
echo toolkitver %toolkitver%
echo toolkitverdist %toolkitverdist%
::echo Copying %toolkitverdist% 
::echo To %toolkitver%
::Copy /v %toolkitverdist% %toolkitver%


echo Checking .git\HEAD exists ...
::github commit version script
set newrev=nongit
if exist %solution%\.git\HEAD (
	echo Reading .git\HEAD ...
	set /p head=<"%solution%\.git\HEAD"
	if "!head:~0,4!" == "ref:" (
		echo Reading .git\!head:~5!...
		if exist "%solution%\.git\!head:~5!" set /p commit=<"%solution%\.git\!head:~5!"
	) else (
		set commit=!head!
	)
	if not "!commit!" == ""	(
		echo Found commit: !commit!
		set newrev=!commit:~0,8!
		echo newrev !newrev!
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
if not "!commit!" == ""	(
	echo Replacing %oldrev% 
	echo In %toolkitver% 
	echo With %newrev% ...
)
::pause

::git version replacement script
for /f "tokens=* delims==" %%i in (%toolkitverdist%) do (
	set str=%%i
	set newstr=!str:%oldrev%=%newrev%!
	echo !newstr!>>tempfile.txt
)

echo Moving new %toolkitver% ...
move /y tempfile.txt "%toolkitver%"
echo Done

pause

endlocal
exit /b 0

:BuildEventFailed
echo Pre-Build Event Failed in prebuild.bat file
pause