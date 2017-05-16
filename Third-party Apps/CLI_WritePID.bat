REM This is an example batch file that can be used for CLI programs

cls
@echo off
SET TP=%transferprofile.exe
SET ACTION=%write
SET FILES=%*

REM  ******  Enter the PID from Computer B on the next line  ******
SET PID="00-00-00-00"

COLOR 0A

%~d0
cd %~p0

echo.
echo. Drag/Drop Computer A 'localprofiles.json' and '_prfldb'
echo. files onto this batch file to write the PID for
echo. Computer B to Computer A files ...
echo.
echo. Remember to set the PID correctly inside the batch file ... 
echo. PID is currently set to: %PID%
echo. 
echo.
PAUSE>NUL|SET /P "= Press any key to contiune or Ctrl-C to escape ..."
echo.
@echo on

%TP% %ACTION% %PID% %FILES% 

@echo off
echo.
REM pause
@echo on

