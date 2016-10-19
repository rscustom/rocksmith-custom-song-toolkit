@ECHO OFF

COLOR 0A

ECHO This batch runs remastered.exe with the -org option
ECHO.
ECHO CDCL with (.org) extensions will be repaired with CLI
ECHO.
ECHO Song stats are preserved only for CDLC that have
ECHO not been played in Rocksmith 2014 Remastered
ECHO.
ECHO If a song has been played and has the 100%% bug then the
ECHO CDLC must be repaired and song stats must be reset using
ECHO remastered.exe with no options or just the [-org] option
ECHO.
ECHO Drag/Drop CDLC files and directories onto this batch
ECHO.
ECHO A shortcut to the batch can be put anywhere

TITLE remastered.exe -org -pre

IF "%~1"=="" GOTO done

cd /d "%~dp0"
START "" "remastered.exe" "-org" "-pre" "%~1"

:done
pause
exit /b
@ECHO ON  