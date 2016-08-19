param([string]$Assembly_File) 

$content = [IO.File]::ReadAllText($Assembly_File)

$regexAIV = new-object System.Text.RegularExpressions.Regex ('(AssemblyInformationalVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$regexAV = new-object System.Text.RegularExpressions.Regex ('(AssemblyVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

# new version
$env:GIT_HASH = $env:APPVEYOR_REPO_COMMIT.Substring(0, 8)
$Assembly_Informational_Version = "$env:GIT_HASH"

# edit the AssemblyVersion here 
# will be applied to all AssemblyInfo.cs files ...
$Assembly_Version = "2.7.1.0"

Write-Host "- Patching: $Assembly_File"
Write-Host "- AssemblyVersion: $Assembly_Version"
Write-Host "- AssemblyInformationVersion: $env:GIT_HASH"

# update assembly info
$content = $regexAIV.Replace($content, '${1}' + $Assembly_Informational_Version + '${4}')
$content = $regexAV.Replace($content, '${1}' + $Assembly_Version + '${4}')

[IO.File]::WriteAllText($Assembly_File, $content)
