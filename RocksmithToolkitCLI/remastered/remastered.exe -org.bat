@ECHO OFF

COLOR 0A

ECHO This batch runs remastered.exe with the -org option
ECHO.
ECHO CDCL with (.org) extensions will be repaired and
ECHO song stats are reset.  Be aware that if this option is
ECHO used on files located in the "My Documents\Remastered_CLI"
ECHO folder that the remastered CDLC needs to be manually move
ECHO back to the Rocksmith 2014 'dlc' folder.  
ECHO.
ECHO Drag/Drop CDLC files and directories onto this batch
ECHO.
ECHO A shortcut to the batch can be put anywhere

TITLE remastered.exe -org

IF "%~1"=="" GOTO done

cd /d "%~dp0"
START "" "remastered.exe" "-org" "%~1"

:done
pause
exit /b
@ECHO ON  