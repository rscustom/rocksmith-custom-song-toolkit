REM This is an example batch file that can be used for CLI programs

cls
@echo off
setlocal ENABLEDELAYEDEXPANSION
rem Take the cmd-line, remove all until the first parameter
set "params=!cmdcmdline:~0,-1!"
set "params=!params:*" =!"
::echo %params%
::pause

set PP=%cdlcconverter.exe
set SOURCE=%Pc
set DESTINATION=%Mac

COLOR 0A
%~d0
cd %~p0

echo.
echo. Drag/Drop CDLC files onto this batch conversion.
echo.
echo. Currently %SOURCE% CDLC songs will be converted to %DESTINATION%
echo. The conversion can be changed inside the batch file.
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.

IF [%1]==[] (
::echo.
::echo. Please drag/drop some files on the batch icon
::PAUSE>NUL|SET /P "= Press any key to contiune ..."
GOTO :b_end
)

rem Split the parameters on spaces but respect the quotes
for %%N IN (!params!) do (
  echo.
  ::echo %%N
  %PP% -s=%SOURCE% -t=%DESTINATION% -i=%%N -appid=248750
)

echo.
PAUSE>NUL|SET /P "= All Done ... Press any key to contiune ..."

:b_end
exit



