@echo off
echo Quickly converts all project files back to VS2010 compatible files
echo.
START /B /WAIT RocksmithDevBuilder.exe CONVERT WAIT
echo.
@echo on