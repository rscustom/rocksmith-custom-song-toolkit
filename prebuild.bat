@echo off
setlocal enabledelayedexpansion
if errorlevel 1 goto BuildEventFailed

set solution=%~1
set toolkitver=%~2

:: if problem passing CL from VS pre-build event then hard code
:: set solution=".\"
:: set toolkitver=".\RocksmithToolkitLib\ToolkitVersion.cs"

::git in 128 char range.
echo Checking for .git\HEAD...
set rev=nongit
if exist "%solution%\.git\HEAD" (
	echo Reading .git\HEAD...
	set /p head=<"%solution%\.git\HEAD"
	if "!head:~0,4!" == "ref:" (
		echo Reading .git\!head:~5!...
		if exist "%solution%\.git\!head:~5!" set /p commit=<"%solution%\.git\!head:~5!"
	) else (
		set commit=!head!
	)
	if not "!commit!" == ""	(
		echo Found commit: !commit!
		set rev=!commit:~0,8!
	) else echo Unable to find commit
)
:: *.cs files in Unicode
echo Reading ToolkitVersion.cs_dist...
set origstr=00000000

echo %toolkitver%_dist
:: next line is required for VS2010 Pre-Build Event to work with WinXP
chcp 65001>nul
::pause

for /f "tokens=* delims=" %%i in ('type "%toolkitver%_dist"') do (
	set str=%%i
	set newstr=!str:%origstr%=%rev%!
	echo !newstr! >> tempfile.txt
)

echo Writing ToolkitVersion.cs...
move /y tempfile.txt "%toolkitver%"
echo Done
exit /b 0

:BuildEventFailed
echo Pre-Build Event Failed in prebuild.bat file
pause
