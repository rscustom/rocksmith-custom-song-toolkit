REM This is an example batch file that can be used for CLI programs

cls
@echo off
SET PP=%packer.exe

COLOR 0A

%~d0
cd %~p0

for %%a in (%1) do set folder=%%~na
::echo.%folder%

echo.
echo. Drag/Drop an unpacked archive folder with subfolders onto this batch file.
echo.
echo. A CDLC song will be created: %folder%_p.psarc
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
@echo on

%PP% -p -f=Pc -v=RS2014 -i=%1 -o=%~d1\Temp\%folder%_p

@echo off
echo.
pause
@echo on

