param([string]$Assembly_File) 

$content = [IO.File]::ReadAllText($Assembly_File)

$regexAIV = new-object System.Text.RegularExpressions.Regex ('(AssemblyInformationalVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$regexAV = new-object System.Text.RegularExpressions.Regex ('(AssemblyVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$regexAC = new-object System.Text.RegularExpressions.Regex ('(AssemblyConfiguration(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)
		 
# new version
$env:GIT_HASH = $env:APPVEYOR_REPO_COMMIT.Substring(0, 8)
$Assembly_Informational_Version = "$env:GIT_HASH"

# NOTE TO DEVS
# Manually edit the '$AssemblyVersion' and '$AssemblyConfiguration' values
# before committing a major revisions/releases to github
# these will be automatically applied to all AssemblyInfo.cs files by AppVeyor ...
# $AssemblyConfiguration should be "BETA" or use "" if RELEASE
$Assembly_Version = "2.8.4.1"
$Assembly_Configuration = "BETA"

Write-Host "- Patching: $Assembly_File"
Write-Host "- AssemblyVersion: $Assembly_Version"
Write-Host "- AssemblyInformationVersion: $env:GIT_HASH"
Write-Host "- AssemblyVersion: $Assembly_Configuration"

# update assembly info
$content = $regexAC.Replace($content, '${1}' + $Assembly_Configuration + '${4}')
$content = $regexAIV.Replace($content, '${1}' + $Assembly_Informational_Version + '${4}')
$content = $regexAV.Replace($content, '${1}' + $Assembly_Version + '${4}')

[IO.File]::WriteAllText($Assembly_File, $content)
