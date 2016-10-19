@ECHO OFF

COLOR 0A

ECHO This batch runs remastered.exe with no options
ECHO.
ECHO CDLC that have the 100%% bug will be repaired
ECHO and Song Stats are reset. The original buggy 
ECHO CDLC is renamed with (.org) file extesion.
ECHO.
ECHO Drag/Drop CDLC files and directories onto this batch
ECHO.
ECHO A shortcut to the batch can be put anywhere

TITLE remastered.exe Repairs 100% Bug and Resets Song Stats

IF "%~1"=="" GOTO done

cd /d "%~dp0"
START "" "remastered.exe" "%~1"

:done
pause
exit /b
@ECHO ON  