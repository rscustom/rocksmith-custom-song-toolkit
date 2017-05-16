REM This is an example batch file that can be used for CLI programs

cls
@echo off
SET TP=%transferprofile.exe
SET ACTION=%read
SET FILE=%1

COLOR 0A

%~d0
cd %~p0

echo.
echo. Drag/Drop the 'localprofiles.json' file onto this batch
echo. to read the PID and copy it to the Clipboard.
echo. 
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
@echo on

%TP% %ACTION% %FILE%

@echo off
echo.
REM pause
@echo on

