@ECHO OFF

COLOR 0A

ECHO This batch runs remastered.exe with the -ren option
ECHO.
ECHO CDCL with (.org) extensions will be renamed to (.psarc)
ECHO This restores the original unrepaired CDLC.  For 
ECHO safety (.org) files are not deleted by the CLI.
ECHO.
ECHO Drag/Drop CDLC files and directories onto this batch
ECHO.
ECHO A shortcut to the batch can be put anywhere

TITLE remastered.exe -ren

IF "%~1"=="" GOTO done

cd /d "%~dp0"
START "" "remastered.exe" "-ren" "%~1"

:done
pause
exit /b
@ECHO ON  