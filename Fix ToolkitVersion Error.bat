REM  Prebuild does not work on some systems so use this bat to before running
REM  VS2010 to rename the ToolkitVersion.cs_dist file so the CSC will build

Copy .\RocksmithToolkitLib\ToolkitVersion.cs_dist .\RocksmithToolkitLib\ToolkitVersion.cs

REM pause