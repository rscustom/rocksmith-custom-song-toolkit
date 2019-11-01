param([string]$Assembly_File) 

$content = [IO.File]::ReadAllText($Assembly_File)

$regexAV = new-object System.Text.RegularExpressions.Regex ('(AssemblyVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$regexAIV = new-object System.Text.RegularExpressions.Regex ('(AssemblyInformationalVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$regexAC = new-object System.Text.RegularExpressions.Regex ('(AssemblyConfiguration(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)
		 
# new version
$env:GIT_HASH = $env:APPVEYOR_REPO_COMMIT.Substring(0, 8)
$Assembly_Informational_Version = "$env:GIT_HASH"

# Manually edit the '$AssemblyVersion', and '$AssemblyConfiguration' values here as needed
# before committing a major new revisions/releases to github after all testing is completed
# these will be automatically applied to the AssemblyInfo.cs files by AppVeyor ...
$Assembly_Version = "2.9.2.1"

# $AssemblyConfiguration = "BUILD", "BETA", "RELEASE", or (any other string) 
# default usage is for sortable ISO8601 DateTime
# $AssemblyConfiguration = $env:APPVEYOR_REPO_COMMIT_TIMESTAMP.Substring(0,19) 
$Assembly_Configuration = $env:APPVEYOR_REPO_COMMIT_TIMESTAMP.Substring(0,19)

# appveyor console output
Write-Host "- Patching: $Assembly_File"
Write-Host "- AssemblyVersion: $Assembly_Version"
Write-Host "- AssemblyInformationVersion: $Assembly_Informational_Version"
Write-Host "- AssemblyConfiguration: $Assembly_Configuration"

# update assembly info
$content = $regexAV.Replace($content, '${1}' + $Assembly_Version + '${4}')
$content = $regexAIV.Replace($content, '${1}' + $Assembly_Informational_Version + '${4}')
$content = $regexAC.Replace($content, '${1}' + $Assembly_Configuration + '${4}')

[IO.File]::WriteAllText($Assembly_File, $content)
