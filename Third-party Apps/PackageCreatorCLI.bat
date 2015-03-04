REM This is an example batch file that can be used to customize packagecreator.exe settings

cls
@echo off
SET PC=%packagecreator.exe

COLOR 0A

%~d0
cd %~p0

echo.
echo. Drag/Drop a directory onto this batch file with CDLC ready files.  
echo.
echo. The files will be packaged into a RS2014 CDLC archive *_p.psarc
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
@echo on

%PC% -p -f=Pc -v=RS2014 -a=248750 -r=1 -q=4 -d=-12 -i=%1 -o=%~d1\Temp

@echo off
echo.
pause
@echo on

