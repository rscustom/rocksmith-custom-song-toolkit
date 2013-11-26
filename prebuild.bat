@echo off
setlocal enabledelayedexpansion

call :dequote %1
set solution=%ret%
call :dequote %2
set toolkitver=%ret%

set rev=nongit
if exist "%solution%\.git\HEAD" (
	set /p head=<"%solution%\.git\HEAD"
	if "!head:~0,4!" == "ref:" (
		if exist "%solution%\.git\!head:~5!" set /p commit=<"%solution%\.git\!head:~5!"
	) else (
		set commit=!head!
	)
	if not "!commit!" == ""	set rev=!commit:~0,8!
)

set origstr=00000000
for /f "tokens=* delims=" %%i in ('type "%toolkitver%_dist"') do (
	set str=%%i
	set newstr=!str:%origstr%=%rev%!
	echo !newstr! >> tempfile.txt
)
move /y tempfile.txt "%toolkitver%"


:dequote
set remquote=%~1
set ret=%remquote%