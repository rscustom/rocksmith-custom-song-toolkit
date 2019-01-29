REM This is an example batch file that can be used for CLI programs

cls
@echo off
SET UP=%packer.exe

COLOR 0A

%~d0
cd %~p0

echo.
echo. Drag/Drop a CDLC packed archive onto this batch file.
echo.
echo. CDLC archive will be unpacked to folder: %~d1\Temp
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
@echo on

%UP% -u -x -d -c -f=Pc -v=RS2014 -i=%1 -o=%~d1\Temp

@echo off
echo.
pause
@echo on

