@echo off
echo Quickly converts all project files back to VS2010 compatible files
echo.
START /B /WAIT RocksmithPreBuild.exe CONVERT "null" "null"
echo.

pause
@echo on